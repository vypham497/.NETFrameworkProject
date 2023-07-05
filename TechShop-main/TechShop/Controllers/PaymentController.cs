using log4net;
using TechShop.Models;
using TechShop.Common;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechShop.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        private TechShopDbContext db = new TechShopDbContext();
        private const string CartSession = "CartSession";
        public double TyGiaUSD = 23003;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
            return View();
        }
        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {

            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Payment/PaymentWithPayPal?";

                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {

                    // This function exectues after receving all parameters for the payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        //return View("FailureView");
                        return Redirect("/chua-hoan-thanh");
                    }
                }
            }
            catch (Exception ex)
            {
                //return View("FailureView");
                return Redirect("/chua-hoan-thanh");
            }

            //on successful payment, show success page to user.
            TechShop.Models.Order order = db.Orders.Find(Session["OrderID"]);
            order.Status = "yes paypal";
            db.SaveChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Client/template/1.cshtml"));
            content = content.Replace("{{CustomerName}}", order.ShipName);
            content = content.Replace("{{Phone}}", order.ShipMobile);
            content = content.Replace("{{Email}}", order.ShipEmail);
            content = content.Replace("{{Address}}", order.ShipAddress);
            content = content.Replace("{{Total}}", order.TotalPrice.Value.ToString("N0"));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendEmail(order.ShipEmail, "Xác nhận đơn hàng mới từ TechShop", content);
            new MailHelper().SendEmail(toEmail, "TechShop", content);
            Session.Remove("OrderID");
            Session.Remove(SessionMember.CartSession);
            //return View("SuccessView");
            return Redirect("/hoan-thanh");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            var ItemLIst = new ItemList() { items = new List<Item>() };
            var cart = (List<CartItem>)Session[CartSession];
            var total = Math.Round(Convert.ToDouble(cart.Sum(x => x.Quantity * x.Product.Price))/ TyGiaUSD, 2);
            foreach (var item in cart)
            {
                    ItemLIst.items.Add(new Item()
                    {
                        name = item.Product.Title,
                        currency = "USD",
                        price = Math.Round(Convert.ToDouble(item.Product.Price) / TyGiaUSD, 2).ToString(),
                        quantity = item.Quantity.ToString(),
                        sku = "sku",
                        tax = "0"
                    });
            }


            var payer = new Payer() { payment_method = "paypal" };
            var paypalOrderId = DateTime.Now.Ticks;
            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total.ToString()
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No
                amount = amount,
                item_list = ItemLIst
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }
    }
}