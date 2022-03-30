var AmChartsGlobalConfigurations = {

    PathToImages: "/vendors/general/amcharts/images/",

    NiceOneSiteHyperlink: "https://www.niceonesa.com",

    AmchartsSiteHyperlink: "[href='https://www.amcharts.com/javascript-charts/']",

    LineColors: { BlueMine: "#1F588A" },

    FillColors: { LinkWater: "#D6E7F2" },

    CategoryAxisColors: { Black: "#fff" },

    ChartTypes: { Serial: "serial", Funnel: "funnel", Pie: "pie" },

    ChartsColors: { Alabaster: "#1dc9b7" },

    LineChart: {

        ValueField: "totalSale",

        CategoryField: "time",

        LegendValueText: "Sale: [[value]] Time: [[category]]"
    },

    FunnelChart: {

        LabelPositions: { Center: "center", Left: "left", Right: "right" },

        BalloonText: "[[title]]: [[value]]",

        OutlineColors: { White: "#FFFFFF" },

        FunelColors: ["#5AB8DF", "#E27683", "#7CC576", "#FFAD62", "#BD8DBF", "#FFFFFF", "#1F588A", "#D6E7F2", "#fff"],

        ValueField: "value",

        TitleField: "name"

    },

    PieChart: {

        LabelText: "[[name]]:[[value]]%",

        BalloonText: "[[nme]]:<br><span><b>[[value]]%</b></span>",

        Colors: ["#638CC0", "#878c91", "#54CEC5", "#b5b3b3", "#efa764"],

        OutlineColors: { White: "#FFFFFF" },

        ValueField: "value",

        TitleField: "name",

        StartEffects: {
            EaseOutSine: "easeOutSine", EaseInSine: "easeInSine", Elastic: "elastic", Bounce: "bounce"
        }
    },
    GraphTypes: { Line: "line", Column: "column", Step: "step", SmoothedLine: "smoothedLine", Candlestick: "candlestick", Ohlc: "ohlc" },

    BalloonDateFormats: { Millisecond: 'JJ:NN:SS', Second: 'JJ:NN:SS', Minute: 'JJ:NN', Hour: 'JJ:NN', Date: 'JJ:NN', Week: 'DD MMM YYYY', Month: 'MMM YYYY', Year: 'YYYY' },

    CategoryAxisDateFormats: [{ period: 'fff', format: 'JJ:NN:SS' }, { period: 'ss', format: 'JJ:NN:SS' }, { period: 'mm', format: 'JJ:NN' }, { period: 'hh', format: 'JJ:NN' }, { period: 'DD', format: 'JJ:NN' }, { period: 'WW', format: 'MMM DD' }, { period: 'MM', format: 'MMM' }, { period: 'YYYY', format: 'YYYY' }],

    CategoryAxisGridPositions: { Start: "start", Middle: "middle" },

    ChartCursorPositions: { Start: "start", Middle: "middle", Mouse: "mouse" },

    GraphPointPositions: { Start: "start", Middle: "middle", End: "end" },

    LegendPositions: { Top: "top", Left: "left", Right: "right", Absolute: "absolute" },

    LegendMarkerTypes: { Square: "square", Circle: "Circle", Diamond: "diamond", TriangleUp: "triangleUp", TriangleDown: "triangleDown", TriangleLeft: "triangleLeft", Bubble: "bubble", Line: "line", None: "none" },

    Themes: { Patterns: "patterns", Default: "default", Chalk: "chalk", Light: "light", Black: "black", None: "none" },

    ValueAxesPositions: { Top: "top", Left: "left", Right: "right", Bottom: "bottom" },

    ReplaceAmChartsUrls: function () {

        $(this.AmchartsSiteHyperlink).attr("href", this.NiceOneSiteHyperlink).html("").css("top", "0px");
    }


};
