### @ZERO_length_null_exists: 是否也将零长度的文件作为不存在的文件？默认不是
FileIO.FileExists <- function(path, ZERO_length_null_exists = FALSE) {

	if (file.exists(path)){
	
		if (ZERO_length_null_exists) {
			return(file.info(path)$size >0) 
		} else {
			return(TRUE);
		}

	} else {
		return(FALSE);
	}

}

### 当目标文件不存在的时候函数会返回-1
FileIO.FileLength <- function(path) {

	if (file.exists(path)) {
		return(file.info(path)$size)
	} else {
		return(-1);
	}

}