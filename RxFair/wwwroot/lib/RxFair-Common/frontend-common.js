function InitMaskInput() {
  // Mask Input
  $.fn.mask && $("[data-mask]").each(function () {
    var a = $(this), b = a.attr("data-mask") || "error...", c = a.attr("data-mask-placeholder") || "X";
    a.mask(b, { "placeholder": c }), a = null;
  });
}

function resetForm(formId) {
  document.getElementById(formId).reset();
  $("#" + formId).parsley().reset();
}
