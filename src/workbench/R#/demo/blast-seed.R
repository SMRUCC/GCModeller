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
	
	if (score > min.score) {
		detail <- paste(detail, "+");
		a[[neighbor]] <- list(
			neighbor = neighbor,
			score = score,
			detail = detail
		);
	}
	
   }
   
   a;
}

let res as list <- lapply(words, w -> seed(w));

# print out results
str(res);