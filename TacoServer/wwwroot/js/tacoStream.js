"use strict";

var connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect()
    .withUrl("/tacoStream").build();

var streamBaud = 8192;
var aldlMessageSize = 69;
var gauges = {};
var fpsElement;
var valuesChanged = true;
var renderStartTime;
var lastUpdate;
var lastRenderStart;

function beginRendering() {
    renderStartTime = window.performance.now();
    console.log(renderStartTime);
    requestAnimationFrame(renderGauges);
}

function updateFps(renderStart) {
    if (lastRenderStart) {
        var delta = renderStart - lastRenderStart;
        var fps = (1000 / delta).toFixed(2);
        if (!lastUpdate || (renderStart - lastUpdate) > 50) {
            fpsElement.text(fps.toString());
            lastUpdate = renderStart;
        }

    }
    lastRenderStart = renderStart;
}
function renderGauges(newtime) {
    requestAnimationFrame(renderGauges);
    if (!valuesChanged)
        return;


    updateFps(newtime);
    Object.keys(gauges).forEach((key, ix) => {
        var g = gauges[key];
        if (g.value != null) {
            g.updateValue(g);
        }
    });
    valuesChanged = false;
}

function loadGauges() {
    loadRadialGauges();
    loadTextGauges();

}

function loadRadialGauges() {
    loadGaugeType('[kendo-type=gauge][gauge-type=radial]',
        function(jq) {
            return jq.data('kendoRadialGauge');
        },
        'radial',
        function(g) {
            g.gauge.value(g.value || 0);
        });
}

function loadTextGauges() {
    loadGaugeType('[gauge-type=text]',
        jq => jq,
        'text',
        function(g) {
            g.gauge.text(g.value || '');
        });
}

function loadGaugeType(selector, mapTo,type, updateValueFn) {
    $(selector)
        .each((ix,el) => {
            var e = $(el);
        gauges[e.data('id')] = {
            gauge: mapTo(e),
            value: null,
            type: type,
            updateValue: updateValueFn
        };
    });
}

$(document).ready(() => {
    fpsElement = $('#fps');
    loadGauges();
    var num = 0;
    connection.start()
        .then(() => {
            connection.invoke("SetGaugeIds", Object.keys(gauges))
                .then(() => {
                    beginRendering();
                    connection.stream("TacoGauges", Object.keys(gauges)).subscribe({
                        next: (item) => {
                            valuesChanged = true;
                            Object.keys(gauges).forEach((key, ix) => {
                                gauges[key].value =item[key];
                            });
                        },
                        complete: () => {
                            var li = document.createElement("li");
                            li.textContent = "Stream completed";
                            document.getElementById("messagesList").appendChild(li);
                            endRender();
                        },
                    });
                })

        })
});
