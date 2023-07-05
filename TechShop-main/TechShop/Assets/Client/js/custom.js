jQuery('<div class="quantity-nav"><div class="quantity-button quantity-up">+</div><div class="quantity-button quantity-down">-</div></div>').insertAfter('.quantity-wrap input');
jQuery('.quantity').each(function () {
    var spinner = jQuery(this),
        input = spinner.find('input[type="number"]'),
        btnUp = spinner.find('.quantity-up'),
        btnDown = spinner.find('.quantity-down'),
        min = input.attr('min'),
        max = input.attr('max');

    btnUp.click(function () {
        var oldValue = parseFloat(input.val());
        if (oldValue >= max) {
            var newVal = oldValue;
        } else {
            var newVal = oldValue + 1;
        }
        spinner.find("input").val(newVal);
        spinner.find("input").trigger("change");
        changeQty();
    });

    btnDown.click(function () {
        var oldValue = parseFloat(input.val());
        if (oldValue <= min) {
            var newVal = oldValue;
        } else {
            var newVal = oldValue - 1;
        }
        spinner.find("input").val(newVal);
        spinner.find("input").trigger("change");
        changeQty();
    });

});


//function changeQty() {
//    var pid = $('#qtyInput').attr('data-id');
//    var quantity = $('#qtyInput').val();
//    var link = "/them-gio-hang?id=" + pid + "&quantity=" + quantity;
//    $("#add-to-cart").attr("href", link);
//};

//$('#add-to-cart').off('click').on('click', function () {
//    $('#success-alert').removeClass('hide');
//    $('#success-alert').delay(1000).slideUp(500);
//});