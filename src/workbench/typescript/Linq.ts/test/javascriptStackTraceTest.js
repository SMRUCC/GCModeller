function callsTop2() {
	
		internal2("aaaaa", false);
	
		function internal2(x,y) {
			Hello2();
		}
	}
	
	function Hello2() {
		console.log(TsLinq.StackTrace.Dump());
	}