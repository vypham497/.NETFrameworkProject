
$(document).ready(function () {
    $.validator.addMethod("validatePassword", function (value, element) {
        return this.optional(element) || /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$/i.test(value);
    }, "Hãy nhập password từ 8 đến 16 ký tự bao gồm chữ hoa, chữ thường và ít nhất một chữ số");

    jQuery.validator.addMethod("notEqual", function (value, element, param) {
        return this.optional(element) || value != $(param).val();
    }, "Mật khẩu phải khác với username");

    $('#frmUser').validate({
        errorClass: 'text-danger animation-slideDown', // You can change the animation class for a different entrance animation - check animations page  
        errorElement: 'div',
        errorPlacement: function (error, e) {
            e.parents('.form-group > div').append(error);
        },
        highlight: function (e) {

            $(e).closest('.form-control').removeClass('is-valid').addClass('is-invalid');
        },
        unhighlight: function (e) {
            $(e).closest('.form-control').removeClass('is-invalid').addClass('is-valid');
            $(e).closest('.text-danger').remove();
        },  
    rules: {
        UserName: {
            required: true,
            maxlength: 100,
            minlength: 6
        },
        Password: {
            required: true,
            validatePassword: "Password",
            maxlength: 16,
            minlength: 8,
            notEqual: "#UserName"
        },
        pwdConfirm : {
            required: true,
            equalTo: '#Password'
        },
        Email: {
            email: true
        }
    },
    messages: {
        UserName: {
            required: "Vui lòng điền username",
            maxlength: "username quá dài",
            minlength: "username quá ngắn"
        },
        Password: {
            required: "Vui lòng điền password",
            maxlength: "password quá dài",
            minlength: "password quá ngắn",
        },
        pwdConfirm: {
            required: "Vui lòng nhập lại password",
            equalTo: "Password không trùng nhau!"
        },
        Email: {
            email: 'Vui lòng điền email hợp lệ'
        },  
    },
    submitHandler: function (form) {
        form.submit();
    }
    });
});
