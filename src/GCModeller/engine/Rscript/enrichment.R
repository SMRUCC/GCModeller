imports "GCModeller" from "GCModeller_cli.dll";

require(dataframe);

let dem <- [?"--data"] :> read.dataframe(mode = "numeric");
let cols <- dem :> dataset.colnames;

console::progressbar.pin.top();

let partition as string;
let data;
let [up, down, all] as string;

for(i in 1:length(cols) step 3) {
    # get foldchange value
    partition <- cols[i+1];
    data <- dem :> dataset.project(cols = partition) :> as.object;
    up <- data :> which(x -> x$GetItemValue(partition) >= 0.1) :> projectAs(x -> x$ID);
    down <- data :> which(x -> x$GetItemValue(partition) <= neg(0.1)) :> projectAs(x -> x$ID);
    all <- up << down;

    print(partition);
}