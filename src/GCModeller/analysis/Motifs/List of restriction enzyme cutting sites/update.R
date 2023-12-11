imports "http" from "webKit";

setwd(@dir);

let main = http::requests.get("https://en.wikipedia.org/wiki/List_of_restriction_enzyme_cutting_sites") |> http::content();
let url = $'[/]wiki[/]List_of_restriction_enzyme_cutting_sites[^"]+';

url = `https://en.wikipedia.org${url(main)}`;

str(main);
print(url);

writeLines(main, con = "./index.txt");

for(file in list.files("./txt")) {
    file.remove(file);
}

for(link in url) {
    const name = basename(link) |> gsub("List_of_restriction_enzyme_cutting_sites:", "");

    print(name);
    # stop();

    link 
    |> http::requests.get() 
    |> http::content()
    |> writeLines(
        con = `./txt/${name}.txt`;
    );
}