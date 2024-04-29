(function() {
  /**
   * Unobtrusive validation support library for jQuery and jQuery Validate
   * Copyright (c) .NET Foundation. All rights reserved.
   * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
   * @version v4.0.0
   */

  // jslint directives
  /*jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, newcap: true, immed: true, strict: false */
  /*global document: false, jQuery: false */

  // dependencies
  const $jQval = jQuery.validator;

  // settings
  const data_validation = "unobtrusiveValidation";

  // helper functions
  const setValidationValues = (options, ruleName, value) => {
    options.rules[ruleName] = value;
    if (options.message) {
      options.messages[ruleName] = options.message;
    }
  };

  const splitAndTrim = value => value.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/g);

  const escapeAttributeValue = value =>
    value.replace(/([!"#$%&'()*+,./:;<=>?@\[\\\]^`{|}~])/g, "\\$1");

  const getModelPrefix = fieldName =>
    fieldName.substr(0, fieldName.lastIndexOf(".") + 1);

  const appendModelPrefix = (value, prefix) =>
    value.indexOf("*.") === 0 ? value.replace("*.", prefix) : value;

  const onError = (error, inputElement) => {
    const container = $(inputElement).closest("[data-valmsg-for]");
    const replaceAttrValue = container.attr("data-valmsg-replace");
    const replace = replaceAttrValue ? JSON.parse(replaceAttrValue) : null;

    container.removeClass("field-validation-valid").addClass("field-validation-error");
    error.data("unobtrusiveContainer", container);

    if (replace) {
      container.empty();
      error.removeClass("input-validation-error").appendTo(container);
    } else {
      error.hide();
    }
  };

  const onErrors = (event, validator) => {
    const container = $(event.currentTarget).find("[data-valmsg-summary=true]");
    const list = container.find("ul");

    if (list && list.length && validator.errorList.length) {
      list.empty();
      container.addClass("validation-summary-errors").removeClass("validation-summary-valid");

      validator.errorList.forEach(error => {
        $("<li />").html(error.message).appendTo(list);
      });
    }
  };

  const onSuccess = (error, inputElement) => {
    const container = error.data("unobtrusiveContainer");

    if (container) {
      const replaceAttrValue = container.attr("data-valmsg-replace");
      const replace = replaceAttrValue ? JSON.parse(replaceAttrValue) : null;

      container.addClass("field-validation-valid").removeClass("field-validation-error");
      error.removeData("unobtrusiveContainer");

      if (replace) {
        container.empty();
      }
    }
  };

  const onReset = event => {
    const $form = $(event.currentTarget);
    const key = "__jquery_unobtrusive_validation_form_reset";

    if ($form.data(key)) {
      return;
    }

    $form.data(key, true);

    try {
      $form.data("validator").resetForm();
    } finally {
      $form.removeData(key);
    }

    $form
      .find(".validation-summary-errors")
      .addClass("validation-summary-valid")
      .removeClass("validation-summary-errors");
    $form
      .find(".field-validation-error")
      .addClass("field-validation-valid")
      .removeClass("field-validation-error")
      .removeData("unobtrusiveContainer")
      .find(">*")
      .removeData("unobtrusiveContainer");
  };

  const validationInfo = form => {
    const $form
