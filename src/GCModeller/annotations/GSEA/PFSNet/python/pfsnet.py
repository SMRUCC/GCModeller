#!/usr/bin/python

'''
Date created: 10/04/2015
Last modified: 07/06/2015
Author: Abha Belorkar
Organization: School of Computing, National University of Singapore
Description: 
This program implements PFSnet as in the paper -- 
	Lim, Kevin, and Limsoon Wong. 
	"Finding consistent disease subnetworks using PFSNet." 
	Bioinformatics, 30(2):189--196, January 2014
'''

import os
import sys, getopt
import numpy as np
from scipy.stats import rankdata
import networkx as nx
from time import time
from numpy.random import choice
from math import sqrt

P_VAL_CUT_OFF = 0.05

# this function prints program usage instructions - command and expected arguments
def usage():
	
	print "\nCALL:\n"
	print "   python pfsnet.py\n\n"
	print "ARGS:\n"
	print "1. --control=<control_expr_file> OR -c <control_expr_file> (compulsory argument)\n"
	print "Absolute/relative path of the file containing expression matrix for control group samples; tab separated text file; matrix: genes x samples (first column: gene ids/names); first line skipped as header.\n\n"
	print "2. --test=<test_expr_file> OR -t <test_expr_file> (compulsory argument)\n"
	print "Absolute/relative path of the file containing expression matrix for test group samples; tab separated text file; matrix: genes x samples (first column: gene ids/names); first line skipped as header.\n\n"
	print "3. --pathway=<pathway_file> OR -p <pathway_file> (compulsory argument)\n"
	print "Tab separated txt file; first line skipped as header; each edge as a row; 3 columns - pathway name<tab>gene_1<tab>gene_2.\n\n"
	print "4. --theta1=<theta1_value> OR -h <theta1_value> (optional; default=0.95)\n"
	print "value between 0 to 1; denotes the quantile threshold above which patient gives full vote to a gene.\n\n"
	print "5. --theta2=<theta2_value> OR -l <theta2_value> (optional; default=0.85)\n"
	print "value between 0 to 1; denotes the quantile threshold below which patient gives zero vote to a gene.\n\n"
	print "6. --beta=<beta_value> OR -b <beta_value> (optional; default=0.5)\n"
	print "value between 0 to 1; denotes the minimum average vote for which the gene is considered highly expressed.\n\n"
	print "7. --permutations=<number_of_permutations> OR -n <number_of_permutations> (optional; default=1000)\n"
	print "denotes the number of points to be generated in the null distribution for permutation test.\n\n"
	sys.exit(2)

# this function accepts arguments, performs necessary checks, assigns defaults
def handle_args (argv):

	# initialization
	fei_c_name = ""
	fei_t_name = ""
	fpw_name = ""
	beta = 0.5
	theta_1 = 0.95
	theta_2 = 0.85
	n_permutations = 1000

	# processing
	try:
		opts, args = getopt.getopt(argv, "c:t:p:b:h:l:n:", ["control=","test=","pathway=","beta=","theta1=","theta2=","permutations="])
	except getopt.GetoptError:
		usage()
		sys.exit(2)
	for opt, arg in opts:
		if opt in ("-c","--control"):
			fei_c_name = arg
		elif opt in ("-t","--test"):
			fei_t_name = arg
		elif opt in ("-p","--pathway"):
			fpw_name = arg
		elif opt in ("-b","--beta"):
			beta = float(arg)
		elif opt in ("-h","--theta1"):
			theta_1 = float(arg)
		elif opt in ("-l","--theta2"):
			theta_2 = float(arg)
		elif opt in ("-n","--permutations"):
			n_permutations = int(arg)
		else:
			print "Unrecognized argument: ", opt, "\n\n"
			usage()

	# handling empty arguments
	if not (fei_c_name):
		print "\nPlease enter the name of control group expression file (with -c).\n"
		usage()
	if not (fei_t_name):
		print "\nPlease enter the name of test group expression file (with -t).\n"
		usage()
	if not (fpw_name):
		print "\nPlease enter the name of pathway file (with -p).\n"
		usage()
	if not (beta < 1 and beta > 0):
		print "\nEnter a beta value between 0 and 1.\n"
		usage()
	if not (theta_1 < 1 and theta_1 > 0):
		print "\nEnter a theta1 value between 0 and 1.\n"
		usage()
	if not (theta_2 < 1 and theta_2 > 0):
		print "\nEnter a theta2 value between 0 and 1.\n"
		usage()	
	if (n_permutations < 10):
		print "\nEnter number of permutations > 10.\n"
		usage()	

	return fei_c_name, fei_t_name, fpw_name, beta, theta_1, theta_2, n_permutations

# returns gene expression matrix
def load_file(fi_name):

	labels, data = [], []
	fi = open(fi_name, 'r')
	header = fi.readline()
	skipped_gcount = 0

	# file --> 2-d array (genes x samples)
	# skip lines (genes) with missing values
	for line in fi:
		ln = line.strip().split('\t')
		row = ln[1:]
		try:
			row = map(float, ln[1:])
			data.append(row)
			labels.append(ln[0])
		except:
			skipped_gcount += 1

	return np.array(data), labels, skipped_gcount

# returns a graph per pathway
def load_pathways(fpw_name):
	
	pw2G = {}

	# populate dict pw2G: key (pathway name), value (gene graph)
	fpw = open(fpw_name, 'rb')
	header = fpw.readline()
	for line in fpw:
		row = line.strip().split('\t')
		if len(row) >= 3:
			pw, node_1, node_2 = row
			if pw not in pw2G:
				pw2G[pw] = nx.Graph()
			pw2G[pw].add_edge(node_1, node_2)
	fpw.close()

	return pw2G

# computes weights of genes in patients
def compute_weights(X, genes, theta_1, theta_2):

	num_genes, num_samples = np.shape(X)

	# function calculating vote to a gene from a patient, given quantiles in the group
	def get_vote(r, q1, q2):
		if r >= q1:
			return 1
		if r >= q2:
			return float(r-q2)/(q1-q2)
		else:
			return 0

	# for each sample, calculate gene expression ranks: output matrix -- samples x genes
	ranks = [rankdata(X[:,j], 'dense')/float(num_genes) for j in range(num_samples)]

	# for each sample, calculate theta_1 and theta_2 quantiles for gene expression
	q1 = np.percentile(ranks, theta_1*100, axis=1)
	q2 = np.percentile(ranks, theta_2*100, axis=1)

	# 'weights' is a dict; gene (key) --> fuzzy vote in all samples (value: list)
	weights = {genes[i]:[get_vote(ranks[j][i], q1[j], q2[j]) for j in range(num_samples)] for i in range(num_genes)}

	return weights


# generates subnets using highly expressed genes
def generate_subnets(pw2G, he_genes):

	subnets = {}

	# use genes to induce subgraphs on pathways
	for pw in pw2G:
		s_count = 0
		pw_subgraph = pw2G[pw].subgraph(he_genes)
		for S in nx.connected_component_subgraphs(pw_subgraph):
			if S.number_of_nodes() >= 5:
				subnets[pw + '_' + str(s_count)] = S
				s_count += 1

	return subnets

# this function computes scores for each patient-subnetwork pair in a given class
def get_subnet_scores (subnets, weights_1, weights_2, num_samples):

	# scores is a dictionary which stores the scores (values) of all patients for each subnet (key)
	scores_1 = {s_name:{} for s_name in subnets}
	scores_2 = {s_name:{} for s_name in subnets}
	subnet_scores = {}
		
	# for each patient:
	# score 1: sum over --> (avg fuzzy vote of gene in type 1) x (fuzzy vote to gene from patient)
	# score 2: sum over --> (avg fuzzy vote of gene in type 2) x (fuzzy vote to gene from patient)

	for s_name in subnets:
		S_nodes = subnets[s_name].nodes()
		mean_weights_1 = {g: np.mean(weights_1[g]) for g in S_nodes}
		mean_weights_2 = {g: np.mean(weights_2[g]) for g in S_nodes}
		for j in range(num_samples):
			scores_1[s_name][j] = sum(mean_weights_1[g] * weights_1[g][j] for g in S_nodes)
			scores_2[s_name][j] = sum(mean_weights_2[g] * weights_1[g][j] for g in S_nodes)

	# to add to t-statistic denominator when variance of difference is zero
	epsilon = 10**(-10)

	# for each sample, get per subnet t-statistic value
	for s_name in subnets:

		s1 = np.array(scores_1[s_name].values())
		s2 = np.array(scores_2[s_name].values())

		# paired t-test
		x = s1 - s2
		mean_x = np.mean(x)
		std_err_x = sqrt(np.var(x)/len(x))

		subnet_scores[s_name] = mean_x/ (std_err_x + epsilon)

	return subnet_scores, scores_1, scores_2

# this function generates subnetworks and calculates their scores over the actual dataset
def pfsnet_core(Xc, Xt, X_genes, pw2G, beta, theta_1, theta_2):

	"""computing weights"""

	print 'generating subnetworks...'

	# get gene weights in 2 patient groups
	weights_c = compute_weights(Xc, X_genes, theta_1, theta_2)
	weights_t = compute_weights(Xt, X_genes, theta_1, theta_2)

	# find genes whose mean vote is greater than beta
	he_genes_c = [g for (g, w) in weights_c.iteritems() if np.mean(w) >= beta]
	he_genes_t = [g for (g, w) in weights_t.iteritems() if np.mean(w) >= beta]
	
	"""subnetwork generation"""

	# extract subnets
	subnets_c = generate_subnets(pw2G, he_genes_c)
	subnets_t = generate_subnets(pw2G, he_genes_t)

	"""subnetwork scoring"""

	print 'scoring subnetworks...'

	# for each sample, get subnet scores
	subnet_scores_c, scores_1_c, scores_2_c = get_subnet_scores (subnets_c, weights_c, weights_t, np.shape(Xc)[1])
	subnet_scores_t, scores_1_t, scores_2_t = get_subnet_scores (subnets_t, weights_t, weights_c, np.shape(Xt)[1])

	return subnets_c, subnets_t, subnet_scores_c, subnet_scores_t

# this function calculates subnet scores over randomized datasets
def pfsnet_iter(Xc, Xt, X_genes, subnets_c, subnets_t, theta_1, theta_2):

	"""computing weights"""

	# get gene weights in 2 phenotypes
	weights_c = compute_weights(Xc, X_genes, theta_1, theta_2)
	weights_t = compute_weights(Xt, X_genes, theta_1, theta_2)

	"""subnetwork scoring"""

	# for each sample, get subnet scores
	subnet_scores_c, scores_1_c, scores_2_c = get_subnet_scores (subnets_c, weights_c, weights_t, np.shape(Xc)[1])
	subnet_scores_t, scores_1_t, scores_2_t = get_subnet_scores (subnets_t, weights_t, weights_c, np.shape(Xt)[1])

	return subnet_scores_c, subnet_scores_t

# pfsnet: generate subnetworks, calculate scores, perform permutation test
def pfsnet(Xc, Xt, X_genes, pw2G, beta, theta_1, theta_2, n_permutations):

	'''subnetwork generation'''

	# generating subnets, computing scores
	subnets_c, subnets_t, subnet_scores_c, subnet_scores_t = pfsnet_core(Xc, Xt, X_genes, pw2G, beta, theta_1, theta_2)

	'''permutation test test'''

	print 'performing permutation test...'
	num_c, num_t = np.shape(Xc)[1], np.shape(Xt)[1]
	seq_c, seq_t = set(range(num_c)), set(range(num_t))
	subnet_scores_null_c, subnet_scores_null_t = {}, {}
	for n in range(n_permutations):
		#'''
		if (n+1) % 100 == 0:
			print 'iter:', (n+1)
		#'''
		# 'choose' samples (sampling without replacement)
		s_choice = choice(num_c+num_t-1, size=num_c, replace=False)
		from_c_to_c = set([s for s in s_choice if s < num_c])
		from_t_to_c = set([s-num_c for s in s_choice if s >= num_c])
		from_c_to_t = seq_c - from_c_to_c
		from_t_to_t = seq_t - from_t_to_c
		Xc_new = np.c_[Xc[:, list(from_c_to_c)], Xt[:, list(from_t_to_c)]]
		Xt_new = np.c_[Xc[:, list(from_c_to_t)], Xt[:, list(from_t_to_t)]]
		subnet_scores_null_c[n], subnet_scores_null_t[n] = pfsnet_iter(Xc_new, Xt_new, X_genes, subnets_c, subnets_t, theta_1, theta_2)

	return subnets_c, subnets_t, subnet_scores_c, subnet_scores_t, subnet_scores_null_c, subnet_scores_null_t

# this function returns significant subnets given subnet scores & null dist.
def get_sgnf_subnets(subnet_scores, subnet_scores_null):

	# dict: contains significant subnets (key) with their p-values (values)
	sgnf_subnets = {}

	for subnet in subnet_scores:
		null_dist = [subnet_scores_null[n][subnet] for n in subnet_scores_null]
		# p-value is calculated as proportion of points in null dist. with a greater score
		if subnet_scores[subnet] > 0:
			p_val = np.mean([(point) > (subnet_scores[subnet]) for point in null_dist])
			if p_val <= P_VAL_CUT_OFF:
	 			sgnf_subnets[subnet] = p_val

	return sgnf_subnets

# filters GE matrix to contain expression of common genes in identical order
def rearrange_gdata(Xc, Xt, Xc_genes, Xt_genes):

	Xc_genes, Xt_genes = list(Xc_genes), list(Xt_genes)
	X_genes = list(set(Xc_genes) & set(Xt_genes))
	genes_ci, genes_ti = [], []

	# get indices for genes common to both groups
	for gene in X_genes:
		genes_ci.append(Xc_genes.index(gene))
		genes_ti.append(Xt_genes.index(gene))
	
	# filter expression matrix to include common genes only
	Xc = Xc[genes_ci]
	Xt = Xt[genes_ti]

	return Xc, Xt, X_genes

# write results to file
def write_to_file (f_path, subnets, sgnf_subnets):

	d_path = f_path.split('/')[0]

	# get path of the directory where script is running
	curr_path = os.path.dirname(os.path.abspath(__file__)) + '/'

	# normalize path as per local OS
	d_path = curr_path + os.path.normpath(d_path)
	f_path = curr_path + os.path.normpath(f_path)

	# create directory if it does not exist
	if not os.path.isdir(d_path):
		os.makedirs(d_path)

	fo = open(f_path, 'wb')
	fo.write('subnet,p_value,genes\n')
	for snet in sgnf_subnets:
		s_genes = ','.join(subnets[snet].nodes())
		fo.write(snet + ',' + str(sgnf_subnets[snet]) + ',' + s_genes + '\n')
	fo.close()

# main function
def main(argv):

	start_time = time()
	
	'''argument handling'''

	fei_c_name, fei_t_name, fpw_name, beta, theta_1, theta_2, n_permutations = handle_args(argv)

	'''loading files'''

	print 'loading input files...'

	# control/normal group
	Xc, Xc_genes, skipped_c = load_file(fei_c_name)
	if (skipped_c):
		print 'Control group: missing data -- skipped %d lines' %(skipped_c)

	# test/disease group
	Xt, Xt_genes, skipped_t = load_file(fei_t_name)
	if (skipped_t):
		print 'Test group: missing data -- skipped %d lines' %(skipped_t)

	# pathways
	pw2G = load_pathways(fpw_name)

	# removing genes not common to test and control
	Xc, Xt, X_genes = rearrange_gdata(Xc, Xt, Xc_genes, Xt_genes)

	# pfsnet
	subnets_c, subnets_t, subnet_scores_c, subnet_scores_t, subnet_scores_null_c, subnet_scores_null_t = pfsnet(Xc, Xt, X_genes, pw2G, beta, theta_1, theta_2, n_permutations)

	# get significant subnets
	sgnf_subnets_c = get_sgnf_subnets(subnet_scores_c, subnet_scores_null_c)
	sgnf_subnets_t = get_sgnf_subnets(subnet_scores_t, subnet_scores_null_t)

	# print results
	write_to_file('pfsnet_results/significant_subnetworks_control.csv', subnets_c, sgnf_subnets_c)
	write_to_file('pfsnet_results/significant_subnetworks_test.csv', subnets_t, sgnf_subnets_t)
	print 'DONE. Results can be found in the \'pfsnet_results\' directory.'

	print 'time elapsed (in seconds):', (time() - start_time)

	return

if __name__ == "__main__":
	main(sys.argv[1:])

