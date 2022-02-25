; (function ($, window, document, undefined) {

    var pluginName = "eventlogs",
        dataKey = "plugin_" + pluginName;

    var Plugin = function (element, options) {

        this.element = element;

        this.options = {};

        this.init(options);
    };

    Plugin.prototype = {
        init: function (options) {
            $.extend(true, this.options, options);

            $("#event-start").hide();
            $("#event-end").hide();

            this.element.on("click", this.options, this.setSystemEvents);

            if (this.options.startEvent) {
                $(this.options.startEvent.tringer).on("click", this.options, this.setStratTime);
            }

            if (this.options.endEvent) {
                $(this.options.endEvent.tringer).on("click", this.options, this.setEndTime);
            }

            if (this.options.startEventFromPrevious) {
                $(this.options.startEventFromPrevious.tringer).on("click", this.options, this.setStratTimeFromPreview);
            }

            if (this.options.endEventFromPrevious) {
                $(this.options.endEventFromPrevious.tringer).on("click", this.options, this.setEndTimeFromPreview);
            }
        },

        setSystemEvents: function (event) {
            Dashmix.layout('header_loader_on');

            $.ajax({
                method: "POST",
                url: event.data.url,
                data: { date: event.data.date }
            }).done(function (msg) {
                Dashmix.layout('header_loader_off');

                if (msg.EventStart && msg.EventStart != "") {
                    var startElm = $("#event-start");
                    startElm.show();
                    startElm.find("span").text(msg.EventStart);
                    event.data.startTime = msg.EventStart;
                }

                if (msg.EventEnd && msg.EventEnd != "") {

                    var endElm = $("#event-end");
                    endElm.show();
                    endElm.find("span").text(msg.EventEnd);
                    event.data.endTime = msg.EventEnd;
                }

            });
            return this;
        },

        setStratTime: function (event) {
            $(event.data.startEvent.field).val(event.data.startTime);
        },

        setEndTime: function (event) {
            $(event.data.endEvent.field).val(event.data.endTime);
        },

        setStratTimeFromPreview: function (event) {
            var time = $(event.data.startEventFromPrevious.tringer).find("span").text();
            $(event.data.startEvent.field).val(time);
            $(event.data.endEvent.field).val(time);
        },

        setEndTimeFromPreview: function (event) {
            var time = $(event.data.endEventFromPrevious.tringer).find("span").text();
            $(event.data.startEvent.field).val(time);
            $(event.data.endEvent.field).val(time);
        }

    };

    /*
     * Plugin wrapper, preventing against multiple instantiations and
     * return plugin instance.
     */
    $.fn[pluginName] = function (options) {

        var plugin = this.data(dataKey);

        // has plugin instantiated ?
        if (plugin instanceof Plugin) {
            // if have options arguments, call plugin.init() again
            if (typeof options !== 'undefined') {
                plugin.init(options);
            }
        } else {
            plugin = new Plugin(this, options);
            this.data(dataKey, plugin);
        }

        return plugin;
    };

}(jQuery, window, document));