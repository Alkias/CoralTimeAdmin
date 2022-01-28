; (function (window, document, $, undefined) {

    'use strict';

    /**
     * Plugin NAMESPACE and SELECTOR
     * @type {String}
     * @api private
     */
    var NAMESPACE = 'EventLogs',
        SELECTOR = '[data-' + NAMESPACE + ']';

    /**
     * Plugin constructor
     * @param {Node} element
     * @param {Object} [options]
     * @api public
     */
    function Plugin(element, options) {
        this.options = $.extend(true, $.fn[NAMESPACE].defaults, options);
        this.$element = $(element);
    }

    /**
     * Plugin prototype
     * @type {Object}
     * @api public
     */
    Plugin.prototype = {
        constructor: Plugin,
        version: '1.0',
        /**
         * Init method
         * @api public
         */
        init: function () {
            $("#event-start").hide();
            $("#event-end").hide();;
            $(document).on("click", "#get-events", function () {

                // Clear old events
                $("#event-start-val").text("").hide();
                $("#event-end-val").text("").hide();;

                Dashmix.layout('header_loader_on');

                $.ajax({
                    method: "POST",
                    url: options.url,
                    data: { date: options.date }
                }).done(function (msg) {
                    Dashmix.layout('header_loader_off');

                    if (msg.EventStart && msg.EventStart != "") {

                        $("#event-start").show();
                        $("#event-start-val").text(msg.EventStart).show();

                    }

                    if (msg.EventEnd && msg.EventEnd != "") {

                        $("#event-end").show();
                        $("#event-end-val").text(msg.EventEnd).show();
                    }

                });
            });

            $(document).on("click", "#event-start", function () {

                $("#FromTime").val($(this).find("#event-start-val").text());
                var data = $("#event-start-val").text();
                $("#FromTime").flatpickr("set", "enable", data);
            });

            $(document).on("click", "#event-end", function () {
                $("#ToTime").val($(this).find("#event-end-val").text());
                var data = $("#event-end-val").text();
                $("#ToTime").flatpickr("set", "enable", data);
            });
        }
        // @todo add methods
    };

    /**
     * jQuery plugin definition
     * @param  {String} [method]
     * @param  {Object} [options]
     * @return {Object}
     * @api public
     */
    $.fn[NAMESPACE] = function (method, options) {
        return this.each(function () {
            var $this = $(this),
                data = $this.data('fn.' + NAMESPACE);
            options = (typeof method === 'object') ? method : options;
            if (!data) {
                $this.data('fn.' + NAMESPACE, (data = new Plugin(this, options)));
            }
            data[(typeof method === 'string') ? method : 'init']();
        });
    };

    /**
     * jQuery plugin defaults
     * @type {Object}
     * @api public
     */
    $.fn[NAMESPACE].defaults = {
        // @todo add defaults
        url: "",
        date:""
    };

    /**
     * jQuery plugin data api
     * @api public
     */
    $(document).on('click.' + NAMESPACE, SELECTOR, function (event) {
        $(this)[NAMESPACE]();
        event.preventDefault();
    });

}(this, this.document, this.jQuery));