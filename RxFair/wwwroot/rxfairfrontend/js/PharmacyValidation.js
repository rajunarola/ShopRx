//------ Pharmacies Form Start
var BillingAddressValue;

$(document).ready(function () {
  BillingAddressValue = false;
  $Stepnumber = "";

  // Smart Wizard
  $('#smartwizard').smartWizard({
    selected: 0,
    theme: 'default',
    transitionEffect: 'fade',
    showStepURLhash: false,
    toolbarSettings: {
      toolbarPosition: 'both',
      toolbarButtonPosition: 'end',
      toolbarExtraButtons: [btnFinish, btnCancel]
    }
  });
});
//Toolbar extra buttons
var btnFinish = $('<button></button>').text('Finish')
  .addClass('btn btn-info')
  .on('click', function () { alert('Finish Clicked'); });
var btnCancel = $('<button></button>').text('Cancel')
  .addClass('btn btn-danger')
  .on('click', function () { $('#smartwizard').smartWizard("reset"); });

$("#smartwizard").on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
  if (stepNumber === 0 && !$('#FirstSection').parsley().validate()) {
    return false;
  }
  else if (stepNumber === 1 && !$('#SecondSection').parsley().validate()) {
    return false;
  }
  else if (stepNumber === 2 && !$('#ThirdSection').parsley().validate()) {
    return false;
  } else if (stepNumber === 2 && $('#ThirdSection').parsley().isValid()) {
    var form = $('.form-section');
    var formData = new FormData();
    var files = $("#exampleFormControlFile99").get(0).files;

    formData.append("License_File", files[0]);
    files = $("#exampleFormControlFile94").get(0).files;
    formData.append("DEA_File", files[0]);
    $.each(form.serializeArray(), function (key, input) {
      formData.append(input.name, input.value);
    });

    $.ajax({
      type: 'POST',
      contentType: false,
      processData: false,
      url: 'newpharmacy/RegisterNewPharmacy?billingAddress=' + BillingAddressValue,
      dataType: 'JSON',
      data: formData,
      success: function (response) {
        if (response.status === 1) {
          RxFair.HandleResponse(response, true, function () {
            location.reload();
          });
        } else {
          RxFair.HandleResponse(response);
        }
      }
    });
  }
  return true;
});

$('#gridCheck2').change(function (e) {
  var controls;
  if ($(this).prop("checked") === true) {
    $('.DiffrentBillingIfo').show();
    controls = $("#BillingInfo").find("[data-parsley-required='false']");
    $(controls).attr("data-parsley-required", "true");

    $("#BillstateId").attr("required", "Select state");
    $('.form-section').parsley('reset');
    BillingAddressValue = true;
  }
  else {
    $('.DiffrentBillingIfo').hide();
    controls = $("#BillingInfo").find("[data-parsley-required='true']");
    $(controls).attr("data-parsley-required", "false");

    $("#BillstateId").removeAttr("required");
    $('.form-section').parsley('reset');
    BillingAddressValue = false;
  }
});

