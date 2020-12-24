var RxFair = new function () {
  this.Filetypes = ["jpg", "jpeg", "png"];
  this.MedicineImageTypes = ["jpg", "jpeg", "png"];
  this.Exceltypes = ["xls", "xlsx", "csv"];
  this.Licencetypes = ["gif", "jpg", "jpeg", "pdf", "doc", "docx"];
  this.DocumentTypes = ["doc", "docx", "html", "htm", "odt", "pdf", "xls", "xlsx", "ods", "ppt", "pptx", "txt"];
  var todo = null;
  var isSubModal = false;
  this.typeModel = {
    Primary: "primary",
    Info: "info",
    Success: "success",
    Warning: "warning",
    Danger: "danger",
    Error: "error"
  };

  this.ColorClass = {
    Teal: "darkgreen",
    Green: "lightgreen",
    Red: "redgreen",
    Yellow: "yellow"
  };
  this.SuccessOk = function (title = "", dialogMsg, yesCallback) {
    title === "" ? "Success" : title;
    try {
      swal({ title: title, text: dialogMsg, icon: this.typeModel.Success }, function (isConfirm) {
        yesCallback();
      });
    } catch (err) {
      alert(dialogMsg);
    }
  };
  this.Success = function (title = "", dialogMsg) {
    title === "" ? "Success" : title;
    try {
      swal({ title: title, text: dialogMsg, icon: this.typeModel.Success });
    } catch (err) {
      alert(dialogMsg);
    }
  };

  this.Notification = function (title = "", dialogMsg) {
    title === "" ? "Info" : title;
    try {
      swal({ title: title, text: dialogMsg, icon: this.typeModel.Info });
    } catch (err) {
      alert(dialogMsg);
    }
  };

  this.Warning = function (title = "", dialogMsg) {
    title === "" ? "Warning" : title;
    try {
      swal({ title: title, text: dialogMsg, icon: this.typeModel.Warning });
    } catch (err) {
      alert(dialogMsg);
    }
  };

  this.Error = function (title = "", dialogMsg) {
    title === "" ? "Error" : title;
    try {
      swal({ title: title, showCancelButton: true, showConfirmButton: false, text: dialogMsg, icon: this.typeModel.Error });
    } catch (err) {
      alert(dialogMsg);
    }
  };

  this.Confirm = function (title = "", type, dialogMsg, confirmButtonClass, confirmButtonText, yesCallback, noCallback) {
    confirmButtonClass === "" ? "danger" : confirmButtonClass;
    swal({
      title: title,
      text: dialogMsg,
      type: type,
      showCancelButton: true,
      confirmButtonClass: "btn-" + confirmButtonClass,
      confirmButtonText: confirmButtonText,
      closeOnConfirm: false
    }, function (isConfirm) {
      if (isConfirm) {
        yesCallback();
        swal.close();
      } else {
        if (typeof noCallback !== "undefined") {
          // safe to use the function
          noCallback();
        }
      }
    });
  };

  this.SwapModalButtons = function () {
    $("button.cancel").before($("button.confirm"));
  };

  this.HandleResponse = function (response, isOk = false, yesCallback = null) {

    //var key = Object.keys(response);
    switch (response.status) {
      case 0:
        this.Error("", response.message);
        return response.data;
      case 1:
        if (isOk)
          this.SuccessOk("", response.message, yesCallback);
        else
          this.Success("", response.message);
        return response.data;
      case 2:
        this.Notification("", response.message);
        return response.data;
      case 3:
        this.Warning("", response.message);
        return response.data;
      default:
        this.Success("", response.message);
        return response.data;
    }
  };
};