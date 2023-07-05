
//var sum = 0;
//$(".qty-input").each(function () {
//    sum += +$(this).val();
//});
//$(".p-count").html(sum);
//$(".head__cart__amount").html(sum);

//$(document).ready(function () {
//    var totalPrice = 0;
//    $(".cart-p-price").each(function () {
//        totalPrice += parseFloat($(this).text().replace(/,/g, ''))
//    });
//    totalPrice = addCommas(totalPrice);
//    $(".price").html(totalPrice);
//});

//function addCommas(nStr) { // pass value to function
//    nStr += '';
//    var x = nStr.split('.'); //split value with .
//    var x1 = x[0];
//    var x2 = x.length > 1 ? '.' + x[1] : '';
//    var rgx = /(\d+)(\d{3})/;
//    while (rgx.test(x1)) { //check  array first value with regular expression
//        x1 = x1.replace(rgx, '$1' + ',' + '$2'); // add comma based on check
//    }
//    return x1 + x2 + " đ"; // return final value
//}
