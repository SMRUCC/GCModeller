# https://github.com/yanlinlin82/blast-seed/blob/master/blast-seed.pl

require(bioseq.patterns);

let min.score as double =  ?"--min-score" || 3.0;
let words     as string = (?"--words"     || stop("No words as seeds...")) :> strsplit(delimiter = ",");
let blosum              = bioseq.blast::blosum(?"--blosum"    || "Blosum-62") :> as.object;

print(words);
print(blosum);

let blosum.base <- blosum$keys[!(blosum$keys in ['B', 'Z', 'X', '*'])];

print(blosum.base);

# calc score and create seeds
let seed as function(w) {
   let len = nchar(w);
   let a = list();
   let preload.seeds <- seeds(len, blosum.base);
   
   print(`have ${length(preload.seeds)} for word: '${w}'`);
   
   for(neighbor in preload.seeds) {
	let score = 0;
	let detail = [];
	
	for(i in 1:len) {
		let v <- blosum$GetDistance(mid(w,i,1),mid(neighbor,i,1));
		score <- score + v;
		detail <- detail << v;
	}
	
	if (score >= min.score) {
		detail <- paste(detail, "+");
		a[[neighbor]] <- list(
			neighbor = neighbor,
			score = score,
			detail = detail
		);
	}	
   }
   
   data.frame(
    word = w,
	neighbor = sapply(a, x -> x$neighbor), 
	score = sapply(a, x -> x$score), 
	detail = sapply(a, x -> x$detail)
   );
}

let res as list <- lapply(words, w -> seed(w));

# print out results
str(res);

let word as string = [];
let neighbor as string = [];
let score as string = [];
let detail as string = [];
let max_score as string = [];

for(w in res) {
	word <- word << w[, "word"];
	neighbor <- neighbor << w[, "neighbor"];
	score <- score << w[, "score"];
	detail <- detail << w[, "detail"];
	max_score <- max_score << rep( max(w[, "score"]) , nrow(w) );
}

write.csv( data.frame(
   word = word, 
   neighbor = neighbor, 
   max_score = max_score,
   score = score, 
   detail = detail
), file = "./blast-seeds.csv");


