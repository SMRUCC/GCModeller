-- Example of the GCModeller assembly script for
-- compile a virtual vell data model file

-- *.vhd GCModeller virtual assembly script
--
-- V virtual cell
-- H habiliment 
-- D dialog

-- build a new virtual cell model from a base model
-- which the base model is named hsa and version label is 
-- 20200929
--
-- if the version label is missing, then the latest model
-- of the specific name "hsa" in your repository will be
-- used
FROM hsa:20200929

-- add meta data for your new model

-- MAINTAINER is a kind of shortcut of the 
-- LABEL maintainer="xxx"
MAINTAINER xieguigang "<xie.guigang@gcmodeller.org>"

-- keyword is a term list that used for registry indexing
-- of your generated virtual cell model
-- these keywords is used for local repository search and 
-- online search.
-- 
-- the quot character can be omit if the keyword contains no
-- white space
-- and each keyword terms should be seperated by the comma
-- symbol 
KEYWORDS hsa,"Human Diseases"

-- the label is apply for add meta data to your virtual
-- cell data model
-- label meta is written in key-string_value pair format
-- the meta data key is not limited on number, andalso 
-- you can add any meta name as you wish 
LABEL version="1.0",day="2020-10-01"
LABEL author="xieguigang <xie.guigang@gcmodeller.org>"
LABEL about="blabla"
LABEL url="https://gcmodeller.org"

-- set compiler environment variables
-- the environment variable could affect some compiler
-- behaviors
ENV name="value"
ENV name2="value2"

-- do virtual cell model modifications

-- example of add enzyme by a specific kegg ortholog id
-- this usually could reshape the metabolic network
-- structure
ADD K00087,K00106,K11177,K11178,K13479,K13480,K13482

-- example of add enzyme by specific a kegg orthology 
-- category.
-- this command will add all enzymes in the specific
-- category
ADD KO:"Human Diseases\Neurodegenerative disease\*"

-- as the same as the ADD command, DELETE command also can
-- accept a list of KO id or category match for removes the
-- specific enzymes from the base model for create a new 
-- model 

-- example of delete enzyme by specific a kegg orthology
-- category.
-- this command will removes all enzymes under the 
-- specific category.
DELETE KO:"Human Diseases\Substance dependence\*"