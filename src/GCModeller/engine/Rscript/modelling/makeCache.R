require(GCModeller);

imports "modeller" from "vcellkit";

# do local cache
modeller::cacheOf.enzyme_kinetics(?"--repo" || stop("a repository path must be specific via `--repo` argument!"));
