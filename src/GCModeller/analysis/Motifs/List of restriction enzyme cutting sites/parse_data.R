imports ["Html", "graphquery"] from "webKit";

require(JSON);

setwd(@dir);

const query = graphquery::parseQuery(readText("./parse_data.txt"));

for(file in list.files("./txt")) {
    const document = Html::parse(readText(file));
    str(graphquery::query(document, query));
    stop();
}

