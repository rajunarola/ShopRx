$(document).ready(function () {
  var isTable = $("table");
  if (isTable.length != 0) {
    datatableDefaultSettings();
  }

  $("body").tooltip({ selector: '[data-toggle="tooltip"]', html: true, trigger: 'hover' });
});

$(document).ajaxStart(function (e) {
  blockPage();
});

$(document).ajaxStop(function () {
  unblockPage();
  if (!$(".table").hasClass("w-100")) {
    $(".table").addClass("w-100");
  }
  var table = $(".dataTables_filter");
  var dataPadding = $(table).parents(".tab-content").parent();
  if (!$(dataPadding).hasClass("data-padding")) {
    $(dataPadding).addClass("data-padding");
  }
  var dataPadding0 = $(table).parents(".row:eq(0)");
  if (!$(dataPadding0).find("div").hasClass("res-p-0")) {
    $(dataPadding0).find("div").addClass("res-p-0");
  }
  $(".dataTables_filter input").unbind();
});

$(document).on("click", ".action", function () {
  hideToolTip();
});

$(document).on("click", ".btn-back", function () {
  window.history.back();
});
/**
 * Parsley validating masked fields
 */
$(document).on("keypress", function (evt) {
  if (evt.isDefaultPrevented()) {
    // Assume that's because of maskedInput
    // See https://github.com/guillaumepotier/Parsley.js/issues/1076
    $(evt.target).trigger("input");
  }
});

function logOutUser() {
  document.getElementById('logoutForm').submit();
}

function ResetPassword(id) {
  $.ajax({
    url: "/Account/GlobalResetPassword",
    data: { id: id },
    type: "GET",
    success: function (response) {
      RxFair.HandleResponse(response);
    },
    error: function (data) {
    }
  });
}

function RemoveUser(id, refreshCallback) {
  $.ajax({
    url: "/Account/RemoveUser",
    data: { id: id },
    type: "POST",
    success: function (response) {
      RxFair.HandleResponse(response);
      if (refreshCallback && (typeof refreshCallback == "function")) {
        refreshCallback();
      }
    },
    error: function (data) {
    }
  });
}

function ViewMedicine(id, isUploaded = false) {
  $("#modalContent").load(`/Account/ViewMedicine?id=${id}&isUploaded=${isUploaded}`, function () {
    modalSize("lg");
    $("#divModal").modal('show');
  });
}

function readImageURL(input, previewId, isMultiple = false) {
  if (isMultiple) {
    $.each(input.files, function (i, item) {
      const reader = new FileReader();

      reader.onload = function (e) {
        const content = `<a href="#"><i class="fa fa-times-circle removeimg" aria-hidden="true"></i><img src="" id="previewImage${i}" alt="" class="img-fluid d-block mt-2 pip">`;
        $("div[data-append-to]").append(content);
        $(previewId + i).attr("src", e.target.result);
      }
      reader.readAsDataURL(item);
    });
  } else {
    if (input.files && input.files[0]) {
      const reader = new FileReader();
      reader.onload = function (e) {
        $(previewId).attr("src", e.target.result);
      }
      reader.readAsDataURL(input.files[0]);
    }
  }
}

function emailConfirmed(data) {
  const iconClass = data ? "check" : "times";
  return `<div><i class="fa fa-2x fa-${iconClass}"></i><div>`;
}

function showImage(path, isRound = true) {
  return `<img src="${path}" class="img-fluid ${isRound ? "rounded-circle" : ""}" alt="">`;
}

function showImageTooltip(path, isRound = true) {

  return `<img src="${path}" class="img-fluid ${isRound ? "rounded-circle" : ""}" alt="" onerror='imgError(this);' data-toggle="tooltip" title="<img src='${path}' class='img-fluid ${isRound ? "rounded-circle" : ""}' onerror='imgErrorForToolTip(this,true);'/>"/>`;
}

function imgError(image, isBig = false) {
  image.onerror = "";
  image.src = isBig ? ImageBroken.path.BigMedicineImage : ImageBroken.path.MedicineImage;
  return true;
}

function imgErrorForToolTip(image, isBig = false) {
  image.onerror = "";
  image.src = isBig ? ImageBroken.path.BigMedicineImage : ImageBroken.path.MedicineImage;
  return true;
}

function statusToggle(id, status, isDisabled = false) {
  var content = `<label class="switch"><input type="checkbox" data-id="{id}" {checked} {disabled}><span class="slider round"></span></label>`;
  content = content.replace("{id}", id);
  content = content.replace("{checked}", status === true ? "checked" : "");
  content = content.replace("{disabled}", isDisabled === true ? "disabled" : "");
  return content;
}

function renderHtml(data) {
  return `<div>${data}<div>`;
}

/***
 * Remove HTML Tags From Render cell content.
 */
function removeHtml(data) {
  return $("<div />").html(data).text();
}

function dataTableReSize() {
  var allDatatables = $("table");
  $.each((allDatatables), function (i, item) {
    var parentDiv = $(item).parent("div");
    if (!$(item).hasClass("custom-tbale")) {
      if (!$(parentDiv).hasClass("over-flow-data")) {
        $(parentDiv).addClass("over-flow-data");
      }
    }
  });
}

function dataParams(title, flag, id) {
  var content = `data-toggle="tooltip" data-original-title="{title}" data-flag="{flag}" data-Id="{id}"`;
  content = content.replace("{title}", title);
  content = content.replace("{flag}", flag);
  content = content.replace("{id}", id);
  return content;
}

function dataTableAction(menu) {
  var content = `<div class=""  title="">
    <button type="button" class="btn dropdown-toggle admin_detail" data-toggle="dropdown">
        <i class="fa fa-circle-o" aria-hidden="true"></i>
        <i class="fa fa-circle-o" aria-hidden="true"></i>
        <i class="fa fa-circle-o" aria-hidden="true"></i>
    </button>
    <div class="dropdown-menu">{MENU}</div>
  </div>`;
  return content.replace("{MENU}", menu);
}

/**
 * Used to control content of type paragraph with first 50 characters only
 * On mouse hover - It displays whole content
 * @param {any} data
 * @param {any} type
 * @param {any} row
 * @param {any} isTooltip
 */
function dtEllipsis(data, type, row, isTooltip = false) {
  if (type === "display" && data !== null && data !== undefined) {
    data = data.replace(/<(?:.|\\n)*?>/gm, "");
    if (data.length > 50) {
      if (isTooltip) {
        return `<span class=\"show-ellipsis\" data-toggle="tooltip" data-placement="auto" data-original-title="${data}">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
      }
      return `<span class=\"show-ellipsis\">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
    } else {
      return data;
    }
  } else {
    return data;
  }
};

function dtPopover(data, type, row, isTooltip = false) {
  if (type === "display" && data !== null && data !== undefined) {
    data = data.replace(/<(?:.|\\n)*?>/gm, "");
    if (data.length > 50) {
      if (isTooltip) {
        return `<span class=\"show-ellipsis\"  data-toggle="popover-click" data-placement="auto" data-content="${data}">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
      }
      return `<span class=\"show-ellipsis\">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
    } else {
      return data;
    }
  } else {
    return data;
  }
};

function redeemMoneyPopover(data, isTooltip = false) {
  if (data !== null && data !== undefined) {
    data = data.replace(/<(?:.|\\n)*?>/gm, "");
    if (data.length > 50) {
      if (isTooltip) {
        return `<span class=\"show-ellipsis\" data-toggle="popover" data-placement="auto" data-original-title="${data} ">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
      }
      return `<span class=\"show-ellipsis\">${data.substr(0, 50)}</span><span class=\"no-show\">${data.substr(50)}</span>`;
    } else {
      return data;
    }
  } else {
    return data;
  }
};

function datatableDefaultSettings() {
  /* // DOM Position key index //
 
      l - Length changing (dropdown)
      f - Filtering input (search)
      t - The Table! (datatable)
      i - Information (records)
      p - Pagination (paging)
      r - pRocessing
      < and > - div elements
      <"#id" and > - div with an id
      <"class" and > - div with a class
      <"#id.class" and > - div with an id and class
 
      Also see: http://legacy.datatables.net/usage/features
      */
  $.extend(true, $.fn.dataTable.defaults, {

    //"dom": '<"top"i>rt<"bottom"flp><"clear">',
    "sDom": "<'dt-toolbar'<'col-xs-12 col-sm-6'f><'col-sm-6 col-xs-12 hidden-xs'l>>" +
      //"r t" +
      "t" +
      "<'dt-toolbar-footer'<'col-sm-6 col-xs-12 hidden-xs'i><'col-xs-12 col-sm-6'p>>",
    "oLanguage": {
      "sSearch": '<span class="input-group-addon"><i class="glyphicon glyphicon-search"></i></span>'
    },
    "language": {
      "emptyTable": "No record found.",
      "searchPlaceholder": "Search records"
      //"processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
    },
    "columnDefs": [
      { "className": "text-center", "targets": "_all" }
    ],
    "order": [[1, ""]],
    "rowCallback": function (row, data, index) {
      $(row).attr("data-RowId", data["id"]);
    },
    "responsive": true,
    "searching": true,
    "autoWidth": true,
    "bServerSide": true,
    "bSearchable": true,
    //"bProcessing": true,
    "ordering": true,
    "bDestroy": true,

  });

  /**
   * Remove Special Characters Or Html Tags From Search String
   */
  var searchDiv = document.createElement("div");
  jQuery.fn.dataTable.ext.type.search.html = function (data) {
    searchDiv.innerHTML = data;
    return searchDiv.textContent ?
      searchDiv.textContent
        .replace(/[áÁàÀâÂäÄãÃåÅæÆ]/g, "a")
        .replace(/[çÇ]/g, "c")
        .replace(/[éÉèÈêÊëË]/g, "e")
        .replace(/[íÍìÌîÎïÏîĩĨĬĭ]/g, "i")
        .replace(/[ñÑ]/g, "n")
        .replace(/[óÓòÒôÔöÖœŒ]/g, "o")
        .replace(/[ß]/g, "s")
        .replace(/[úÚùÙûÛüÜ]/g, "u")
        .replace(/[ýÝŷŶŸÿ]/g, "n") :
      searchDiv.innerText.replace(/[üÜ]/g, "u")
        .replace(/[áÁàÀâÂäÄãÃåÅæÆ]/g, "a")
        .replace(/[çÇ]/g, "c")
        .replace(/[éÉèÈêÊëË]/g, "e")
        .replace(/[íÍìÌîÎïÏîĩĨĬĭ]/g, "i")
        .replace(/[ñÑ]/g, "n")
        .replace(/[óÓòÒôÔöÖœŒ]/g, "o")
        .replace(/[ß]/g, "s")
        .replace(/[úÚùÙûÛüÜ]/g, "u")
        .replace(/[ýÝŷŶŸÿ]/g, "n");
  };

  $(document).on("change", ".dataTables_filter input", function () {
    var searchValue = $(this).val();
    var parentTableId = $(this).parents(".dataTables_wrapper").find("table").attr("id");
    // Remove Special Characters Or Html Tags From Search String
    searchValue = jQuery.fn.DataTable.ext.type.search.html(searchValue);
    $("#" + parentTableId).DataTable().search(searchValue).draw();
  });
}

function refreshDatatable(tableId, rowId) {
  $(tableId).DataTable().row(`[data-RowId='${rowId}']`).remove().draw(false);
}

function serializeformToFormData(formName) {
  var formInput = $(formName).serializeArray();
  var formData = new FormData();
  $.each(formInput, function (index, item) {
    formData.append(item.name, item.value);
  });
  return formData;
}

function resetForm(formId) {
  document.getElementById(formId).reset();
  $("#" + formId).parsley().reset();
}

function modalSize(size) {
  var modelSizeClass;
  switch (size) {
    case "lg":
      modelSizeClass = "modal-lg";
      break;
    case "slg":
      modelSizeClass = "modal-slg";
      break;
    case "xl":
      modelSizeClass = "modal-xl";
      break;
    case "md":
      modelSizeClass = "modal-md";
      break;
    default:
      modelSizeClass = "";
      break;
  }
  $("#divModal .modal-dialog").addClass(modelSizeClass);
}
function closeModal() {
  $("#modalContent").html("");
  $("#divModal").modal("hide");
}

function geturlId(url) {
  return url.substring(url.lastIndexOf("/") + 1);
}

function getUrlFields(url, noOf) {
  var all = url.split("/");
  if (all.length >= noOf) {
    return all[noOf];
  } else {
    return "";
  }
}
function getLastUrlFields(url) {
  if (url === undefined) {
    url = window.location.href;
  }
  var all = url.split("/");
  return all[all.length - 1];
}

function returnValue(arrayList, value) {
  var result = 0;
  $.each(arrayList, function (key, val) {
    if (val.Text === value)
      result = parseInt(val.Value);
  });
  return result;
}

function validateUrl(value) {
  return /^(ftp|https?):\/\/+(www\.)?[a-z0-9\-\.]{3,}\.[a-z]{3}$/.test(value);
  //return /^(?:(?:(?:https?|ftp):)?\/\/)(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:[/?#]\S*)?$/i.test(value);
}

function blockElement(selector) {
  $(selector).block({
    message: '<img src = "/rxfairbackend/images/ajax-loader1.gif" class="block-image" /><span class="block-message">Processing...</span>',
    css: {
      width: "auto",
      border: "none !important",
      "border-radius": "5px",
      padding: "6px",
      left: "45.2%"
    }
  });
}

function unblockElement(selector) {
  $(selector).unblock();
}

function blockPage() {
  $.blockUI(
    {
      baseZ: 9999,
      message: '<img src = "/rxfairbackend/images/ajax-loader1.gif" class="block-image"/><span class="block-message">Processing...</span>',
      css: {
        width: "auto",
        border: "none !important", "border-radius": "5px",
        padding: "6px",
        left: "45.2%"
      }
    });
}

function unblockPage() {
  $.unblockUI();
}

function applyScroll() {
  $("table").parents(".dataTables_wrapper").slimscroll({ height: "467px" });
}

function hideToolTip() {
  $('[data-toggle="tooltip"]').tooltip("hide");
}

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function componentToHex(c) {
  var hex = c.toString(16);
  return hex.length === 1 ? "0" + hex : hex;
}

function getRandomColor() {

  //generate random red, green and blue intensity
  var r = getRandomInt(0, 255);
  var g = getRandomInt(0, 255);
  var b = getRandomInt(0, 255);

  var color = "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
  //alert(color);
  return color;

}

function IsEmptyString(value) {
  return value === null || value === "" || value === undefined ? "" : value;
}

function SetActiveMenu(areaName) {
  var url = getLastUrlFields(window.location.href);
  if (url === areaName) {
    url = "";
  }

  $("aside li").find(".active").removeClass("active");
  $("aside li.treeview-menu").find(".is-expanded").removeClass("is-expanded");

  var movepossion = recursiveMenu(url);

  setTimeout(function () {
    $(movepossion).focus();
  }, 100);
}

function recursiveMenu(url) {
  const activeLink = $(`aside li a[mhref="${url}"]`);
  var movepossion;
  if (activeLink.length !== 0) {
    const parentBlock = $(activeLink).parents("ul.treeview-menu");

    if (parentBlock.length !== 0) {
      const childBlock = $(parentBlock).find(`a[mhref="${url}"]`);
      $(childBlock).addClass("active");
      $(parentBlock).parent().addClass("is-expanded");
      movepossion = childBlock;
    } else {
      $(activeLink).addClass("active");
      movepossion = activeLink;
    }
  } else {
    const link = $(".breadcrumb a[mhref]").attr("mhref");
    movepossion = recursiveMenu(link);
  }
  return movepossion;
}

function checkFileStatus(url) {
  return $.ajax({
    url: `/Account/CheckFileExist?url=${url}`,
    type: "GET",
    dataType: "json",
    traditional: true,
    contentType: "application/json; charset=utf-8"
  });
}

function DownloadFile(url) {
  checkFileStatus(url).done(function (response) {
    if (response.status == 0) {
      RxFair.HandleResponse(response);
    } else {
      window.location = `/Account/DownloadFile?url=${response.data}`;
    }
  });
}

function validateEmail(elementValue) {
  var emailPattern = /^[a-zA-Z0-9._+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
  return emailPattern.test(elementValue);
}

function isNumberKey(evt, element) {
  const charCode = (evt.which) ? evt.which : event.keyCode;
  if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charCode == 8))
    return false;
  else {
    const len = $(element).val().length;
    const index = $(element).val().indexOf(".");
    if (index > 0 && charCode == 46) {
      return false;
    }
    if (index > 0) {
      const charAfterdot = (len + 1) - index;
      if (charAfterdot > 3) {
        return false;
      }
    }

  }
  return true;
}

// ReSharper disable once NativeTypePrototypeExtending
String.prototype.ucfirst = function () {
  return this.charAt(0).toUpperCase() + this.substr(1);
}

// ReSharper disable once NativeTypePrototypeExtending
String.prototype.CapitalizeFirstLetter = function () {
  return this.charAt(0).toUpperCase() + this.slice(1);
}

// ReSharper disable once NativeTypePrototypeExtending
Number.prototype.currencyFormat = function () {
  return `$${formatMoney(this)}`;
}

function arrayRemove(array, element) {
  var index = array.indexOf(element);
  array.splice(index, 1);
  return array;
}
function formatMoney(amount, decimalCount = 2, decimal = ".", thousands = ",") {
  try {
    decimalCount = Math.abs(decimalCount);
    decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

    const negativeSign = amount < 0 ? "-" : "";

    const i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
    const j = (i.length > 3) ? i.length % 3 : 0;

    return negativeSign + (j ? i.substr(0, j) + thousands : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, `$1${thousands}`) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
  } catch (e) {
    console.log(e);
  }
};
function bindIcon(url) {
  return ("https://s2.googleusercontent.com/s2/favicons?domain=" + url);
}

var ImageBroken = new function () {
  this.path = {
    RedeemProduct: "/rxfairbackend/images/product.jpg",
    UserProfile: "/rxfairbackend/images/user.png",
    MedicineImage: "/UploadFile/MedicineImage/medi.png",
    BigMedicineImage: "/UploadFile/MedicineImage/medi-big.png"
  };

  this.fixBrokenImages = function (url) {
    const img = $("main").find("img");
    const l = img.length;
    for (let i = 0; i < l; i++) {
      const t = img[i];
      if (t.naturalWidth === 0) {
        //this image is broken
        t.src = url;
      }
    }
  }
  this.fixBrokenUserProfile = function (url) {
    const img = $("header").find("img");// document.getElementsByTagName('img');
    const l = img.length;
    for (let i = 0; i < l; i++) {
      const t = img[i];
      if (t.naturalWidth === 0) {
        //this image is broken
        t.src = url;
      }
    }
  }
}

function InitMaskInput() {
  // Mask Input
  $.fn.mask &&
    $("[data-mask]").each(function () {
      var a = $(this), b = a.attr("data-mask") || "error...", c = a.attr("data-mask-placeholder") || "X";
      a.mask(b, { "placeholder": c }), a = null;
    });
}

