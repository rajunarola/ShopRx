(function () {
	"use strict";

	var treeviewMenu = $('.app-menu');

	// Toggle Sidebar
	$('[data-toggle="sidebar"]').click(function(event) {
		event.preventDefault();
		$('.app').toggleClass('sidenav-toggled');
	});

	// Activate sidebar treeview toggle
	$("[data-toggle='treeview']").click(function(event) {
		event.preventDefault();
		if(!$(this).parent().hasClass('is-expanded')) {
			treeviewMenu.find("[data-toggle='treeview']").parent().removeClass('is-expanded');
		}
		$(this).parent().toggleClass('is-expanded');
	});

	// Set initial active toggle
	$("[data-toggle='treeview.'].is-expanded").parent().toggleClass('is-expanded');

	//Activate bootstrip tooltips
	$("[data-toggle='tooltip']").tooltip();

})();


// ----------- pharmacies Start
function readURL(input) {
	if (input.files && input.files[0]) {
		var reader = new FileReader();
		reader.onload = function(e) {
			$('#imagePreview').css('background-image', 'url('+e.target.result +')');
			$('#imagePreview').hide();
			$('#imagePreview').fadeIn(650);
		}
		reader.readAsDataURL(input.files[0]);
	}
}
$("#imageUpload").change(function() {
	readURL(this);
});
// ----------- pharmacies End

// ---------- Account Start
function reset_acc() {
	$('.ac-title').removeClass('acc-active');
	$('.accordian-para').slideUp();
	$('.plus-icon').removeClass('cross-icon');
}
$('.ac-title').click(function (e) {
	e.preventDefault();
	if ($(this).hasClass('acc-active'))
	{
		reset_acc();
	}
	else {
		reset_acc();
		var getID = $(this).attr('data-in');
		$(getID).slideDown();
		$(this).addClass('acc-active');
		$(this).find('.plus-icon').addClass('cross-icon');
	}
});
// ---------- Account End

// ----------- Admin Div Hide-Show Start
function myFunction() {
	var element = document.getElementById("dic");
	element.classList.toggle("d_main");
}
// ----------- Admin Div Hide-Show End

//------ Pharmacies Form Start
$(document).ready(function(){

	// Step show event
	$("#smartwizard").on("showStep", function(e, anchorObject, stepNumber, stepDirection, stepPosition) {
		//alert("You are on step "+stepNumber+" now");
		if(stepPosition === 'first'){
			$("#prev-btn").addClass('disabled');
		}else if(stepPosition === 'final'){
			$("#next-btn").addClass('disabled');
		}else{
			$("#prev-btn").removeClass('disabled');
			$("#next-btn").removeClass('disabled');
		}
	});

	// Toolbar extra buttons
	var btnFinish = $('<button></button>').text('Finish')
		.addClass('btn btn-info')
		.on('click', function(){ alert('Finish Clicked'); });
	var btnCancel = $('<button></button>').text('Cancel')
		.addClass('btn btn-danger')
		.on('click', function(){ $('#smartwizard').smartWizard("reset"); });


	// Smart Wizard
	$('#smartwizard').smartWizard({
		selected: 0,
		theme: 'default',
        transitionEffect: 'fade',
        showStepURLhash: false,
		toolbarSettings: {toolbarPosition: 'both',
			toolbarButtonPosition: 'end',
			toolbarExtraButtons: [btnFinish, btnCancel]
		}
	});


	// External Button Events
	$("#reset-btn").on("click", function() {
		// Reset wizard
		$('#smartwizard').smartWizard("reset");
		return true;
	});

	$("#prev-btn").on("click", function() {
		// Navigate previous
		$('#smartwizard').smartWizard("prev");
		return true;
	});

	$("#next-btn").on("click", function() {
		// Navigate next
		$('#smartwizard').smartWizard("next");
		return true;
	});

	$("#theme_selector").on("change", function() {
		// Change theme
		$('#smartwizard').smartWizard("theme", $(this).val());
		return true;
	});

	// Set selected theme on page refresh
	$("#theme_selector").change();
});
//------ Pharmacies Form Start



//------------ Testimonial Slider Start -----------
var testimonial_1 = $(".testimonials_sec");
testimonial_1.owlCarousel({
	center: false,
	nav: true,
	autoplayTimeout:6000,
	autoplaySpeed: 3000,
	dots: false,
	items: 3,
	navText: ['<span  class="fa fa-angle-left" ></span>',
		'<span class="fa fa-angle-right" ></span>']
});
//------------ Testimonial Slider End -----------

//---------- Main Slider Start -----------

//---------- Main Slider End -----------

