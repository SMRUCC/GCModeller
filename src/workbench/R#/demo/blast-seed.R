# https://github.com/yanlinlin82/blast-seed/blob/master/blast-seed.pl

let blosum    as string =  ?"--blosum"    || stop("No matrix data provides");
let min.score as double =  ?"--min-score" || 3.0;
let words     as string = (?"--words"     || stop("No words as seeds...")) :> strsplit(delimiter = ",");

print(words);

# calc score and create seeds
let seed as function(w) {

}

let res as list <- lapply(words, w -> seed(w));

# print out results
