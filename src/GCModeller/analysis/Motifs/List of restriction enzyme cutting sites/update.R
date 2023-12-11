imports "http" from "webKit";

setwd(@dir);

let main = http::requests.get("https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites") |> http::content();

print(main);