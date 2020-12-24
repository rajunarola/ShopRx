//------ Pharmacies Form Start
var BillingAddressValue;

$(document).ready(function () {
  BillingAddressValue = false;

  // Smart Wizard
  $("#smartwizard").smartWizard({
    selected: 0,
    theme: "default",
    keyNavigation: false,
    transitionEffect: "fade",
    showStepURLhash: false,
    toolbarSettings: {
      toolbarPosition: "both",
      toolbarButtonPosition: "end",
      toolbarExtraButtons: [btnFinish, btnCancel]
    }

  });
});
//Toolbar extra buttons
var btnFinish = $("<button></button>").text("Finish")
  .addClass("btn btn-info")
  .on("click", function () { alert("Finish Clicked"); });
var btnCancel = $("<button></button>").text("Cancel")
  .addClass("btn btn-danger")
  .on("click", function () { $("#smartwizard").smartWizard("reset"); });

$("#smartwizard").on("showStep", function (e, anchorObject, stepNumber) {
    if (stepNumber === 0 || stepNumber === 3)
        $(".sw-btn-prev").hide();  
    else if (stepNumber === 2)
    $(".sw-btn-next").prop("value", "Finish");
});

$("#smartwizard").on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
  if (stepDirection === "backward") {
    var formName = "";
    switch (stepNumber) {
      case 0:
        formName = "#FirstSection";
        break;
      case 1:
        formName = "#SecondSection";
        break;
      case 2:
        formName = "#ThirdSection";
        break;
      default:
        break;
    }
    $(formName).parsley().reset();
    return true;
  }
  else if (stepNumber === 0 && !$("#FirstSection").parsley().validate()) {
    return false;
  }
  else if (stepNumber === 0 && $("#FirstSection").parsley().validate()) {
      $(".sw-btn-prev").show(function () {
          //tva-set focus on tab first element
          $("input[name=FirstName]").focus();
      });
    return true;
  }
 
  else if (stepNumber === 1 && stepDirection === "forward") {
      //$("input[name=LicenseNumber]").focus();

    $("#SecondSection").parsley().reset();
    var secondIsValid = $("#SecondSection").parsley().validate();
     
    if (result === 0) {
        if (secondIsValid === null || secondIsValid) {   
            return true;            
      }
      else {
        return false;
      }
    }
    else
      return false;
  }

  else if (stepNumber === 1) {

    let boolean = !$("#SecondSection").parsley.whenValidate().fail(function () { return false; });
    return boolean;
  } else if (stepNumber === 2 && !$("#ThirdSection").parsley().validate()) {
    return false;
  }
  else if (stepNumber === 2 && $("#ThirdSection").parsley().validate()) {

    $(".sw-btn-next").hide();
    //disabling Navigation Link of smartwizard
    var temp = $("#smartwizard ul").children('li').children('a');

    var form = $(".form-section");
    var formData = new FormData();
    formData.append("__RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
    var licenseFiles = $("#LicenseFile").get(0).files;

    formData.append("LicenseFile", licenseFiles[0]);
    var deaFiles = $("#DeaFile").get(0).files;
    formData.append("DeaFile", deaFiles[0]);
    $.each(form.serializeArray(), function (key, input) {
      formData.append(input.name, input.value);
      });
    addNewPharmacy(formData, BillingAddressValue).done(function (response) {
      RxFair.HandleResponse(response);
      if (response.status === 1) {
        return true;
      } else {
        return false;
      }
    });
    return true;
  }
  return true;
});

function addNewPharmacy(formData, isBilling) {
  return $.ajax({
    type: "POST",
    contentType: false,
    processData: false,
    url: "/NewPharmacy/RegisterNewPharmacy?isBillingAddress=" + isBilling,
    dataType: "JSON",
    traditional: true,
    data: formData
  });
}

$("#gridCheck2").change(function (e) {
  var controls;
  if ($(this).prop("checked") === true) {
    $(".DiffrentBillingIfo").show();
    controls = $("#BillingInfo").find("[data-parsley-required='false']");
    $(controls).attr("data-parsley-required", "true");

    $("#BillState").attr("required", "Select state");
    $(".form-section").parsley("reset");
    BillingAddressValue = true;
  }
  else {

    $(".DiffrentBillingIfo").hide();
    controls = $("#BillingInfo").find("[data-parsley-required='true']");
    $(controls).attr("data-parsley-required", "false");

    $("#BillState").removeAttr("required");
    $(".form-section").parsley("reset");
    BillingAddressValue = false;
  }

});