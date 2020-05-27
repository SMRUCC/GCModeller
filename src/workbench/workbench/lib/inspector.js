/// <reference path="../build/linq.d.ts" />
function inspect_table(id) {
    var $obj = localStorage.getItem("inspect_table");
    $ts.getText("http://127.0.0.1:7452/inspect?guid=" + $obj, function (str) {
        var objects = $ts.csv.toObjects(str).ToArray();
        localStorage.removeItem("inspect_table");
        $ts.appendTable(objects, id, null, { class: "table" });
    });
}
function inspect_api(id) {
    var $obj = localStorage.getItem("inspect_api");
    $ts.getText("http://127.0.0.1:7452/inspect?guid=" + $obj, function (html) {
        $ts(id).display(html);
    });
}
//# sourceMappingURL=inspector.js.map