// ----------- Nav Active Start -----------------
var header = document.getElementById("navbarNav");
var btns = header.getElementsByClassName("nav-link");
for (var i = 0; i < btns.length; i++) {
    btns[i].addEventListener("click", function () {
        var current = document.getElementsByClassName("active");
        current[0].className = current[0].className.replace(" active", "");
        this.className += " active";
    });
}
// ----------- Nav Active End -----------------

//---------- Main Slider Start -----------
var main_1 = $(".main_slider");
main_1.owlCarousel({
    autoplay: true,
    center: true,
    loop: true,
    autoplayTimeout: 6000,
    autoplaySpeed: 3000,
    dots: false,
    items: 1
});
//---------- Main Slider End -----------

//------------ Testimonial Slider Start -----------
var testimonial_1 = $(".testimonials_sec");
testimonial_1.owlCarousel({
    autoplay: false,
    center: true,
    loop: true,
    nav: true,
    autoplayTimeout: 6000,
    autoplaySpeed: 3000,
    dots: false,
    items: 1,
    navText: ['<span  class="fa fa-angle-left" ></span>',
        '<span class="fa fa-angle-right" ></span>']
});
//------------ Testimonial Slider End -----------

// 2. Tap to Top

$(window).on('scroll', function () {
    if ($(this).scrollTop() > 500) {
        $('.tap-top').fadeIn();
    } else {
        $('.tap-top').fadeOut();
    }
});
$('.tap-top').on('click', function () {
    $("html, body").animate({
        scrollTop: 0
    }, 600);
    return false;
});

/*=====================
3. Nav Bar Fixed
==========================*/
$(window).scroll(function () {
    var sticky = $('.navbar'),
        scroll = $(window).scrollTop();

    if (scroll >= 200) {
        sticky.addClass('fixed');
    }
    else {
        sticky.removeClass('fixed');
    }
});

//code has been moved pharmacyValidation file

//function leaveAStepCallback(obj, context) {
//    alert("Leaving step " + context.fromStep + " to go to step " + context.toStep);
//    return validateSteps(context.fromStep); // return false to stay on step and true to continue navigation 
//}

//// Your Step validation log
//ic
//function validateSteps(stepnumber) {
//    var isStepValid = true;
//    // validate step 1
//    if (stepnumber == 1) {
//        // Your step validation logic
//        // set isStepValid = false if has errors
//    }
//    return isStepValid;
//}

//code has move to Pharmacy

$("#smartwizard").on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
    if (stepNumber === 0 && !$('#FirstSection').parsley().validate()) {
        return false;
    }
    return true;
});

$("#smartwizard").on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
    if (stepNumber === 0 && !$('#FirstSection').parsley().validate()) {
        return false;
    }
    return true;
});

// Step show event

//$("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection, stepPosition) {
//    //console.log('step number ' + stepNumber);
//    //console.log('stepPosition ' + stepPosition);
//    if (stepNumber == 0) {

$("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection, stepPosition) {

    if (stepNumber === 0) {
        $("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection, stepPosition) {
            //Toolbar extra buttons
            var btnFinish = $('<button></button>').text('Finish')
                .addClass('btn btn-info')
                .on('click', function () { alert('Finish Clicked'); });
            var btnCancel = $('<button></button>').text('Cancel')
                .addClass('btn btn-danger')
                .on('click', function () { $('#smartwizard').smartWizard("reset"); });
        });
    }
});
//------ FAQ Start ----------

function reset_acc() {
    $('.ac-title').removeClass('acc-active');
    $('.accordian-para').slideUp();
    $('.plus-icon').removeClass('cross-icon');
}

$(document).on("click", ".ac-title", function (e) {
    if ($(e.currentTarget).hasClass('acc-active')) {
        reset_acc();
    }
    else {
        reset_acc();
        var getId = $(e.currentTarget).attr('data-in');

        $('#tab_' + getId).slideDown();

        $(e.currentTarget).addClass('acc-active');
        $(e.currentTarget).find('.plus-icon').addClass('cross-icon');
    }
});
//------ FAQ End ----------
