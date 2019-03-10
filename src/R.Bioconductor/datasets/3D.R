# Usage examples:
#   Rotate3D(matrix(sample(100), c(10, 10)), col = "lightblue");
#   Rotate3D(outer(-10:10, -10:10, function(x, y) { r <- sqrt(x * x + y * y); 10 * sin(r) / r}), col = "lightblue");

Rotate3D <- function(x, ...) {

  run.Rotate3D <- function(
    x = seq(0, 1, length.out = nrow(z)),
    y = seq(0, 1, length.out = ncol(z)),
    z, theta = 0, phi = 15, verbose = FALSE, step = 5, speed = 20, ...) {

    currentTheta <- theta;
    currentPhi <- phi;
    startX <- NA;
    startY <- NA;
    currentX <- NA;
    currentY <- NA;

    refresh.screen <- function() {
      dTheta <- 0;
      dPhi <- 0;
      if ( ! is.na(startX) && ! is.na(startY) && ! is.na(currentX) && ! is.na(currentY)) {
        dTheta <- -step * (currentX - startX) / (diff(range(x)) / speed);
        dPhi <- -step * (currentY - startY) / (diff(range(y)) / speed);
        if (is.infinite(dTheta)) dTheta <- 0;
        if (is.infinite(dPhi)) dPhi <- 0;
      }
      if (verbose) cat("persp( theta = ", currentTheta + dTheta, ", phi = ", currentPhi + dPhi, ")\n");
      persp(x, y, z, theta = currentTheta + dTheta, phi = currentPhi + dPhi, ...);
    }
  
    eventMouseDown <- function(buttons, eventX, eventY) {
      pointX <- grconvertX(eventX, "ndc", "user")
      pointY <- grconvertY(eventY, "ndc", "user")
      if (verbose) cat("eventMouseDown( buttons = ", buttons, ", pointX = ", pointX, ", piontY = ", pointY, ")\n");
      startX <<- pointX;
      startY <<- pointY;
      NULL;
    }
  
    eventMouseMove <- function (buttons, eventX, eventY) {
      pointX <- grconvertX(eventX, "ndc", "user")
      pointY <- grconvertY(eventY, "ndc", "user")
      if (verbose) cat("eventMouseMove( buttons = ", buttons, ", pointX = ", pointX, ", pointY = ", pointY, ")\n");
      currentX <<- pointX;
      currentY <<- pointY;
      refresh.screen();
      NULL;
    }
  
    eventMouseUp <- function(buttons, eventX, eventY) {
      pointX <- grconvertX(eventX, "ndc", "user")
      pointY <- grconvertY(eventY, "ndc", "user")
      if (verbose) cat("eventMouseUp( buttons = ", buttons, ", pointX = ", pointX, ", pointY = ", pointY, ")\n");
      if ( ! is.na(startX) && ! is.na(startY)) {
        dTheta <- step * (pointX - startX) / (diff(range(x)) / speed);
        dPhi <- step * (pointY - startY) / (diff(range(y)) / speed);
        if (is.finite(dTheta)) currentTheta <<- currentTheta - dTheta;
        if (is.finite(dPhi)) currentPhi <<- currentPhi - dPhi;
      }
      startX <<- NA;
      startY <<- NA;
      currentX <<- NA;
      currentY <<- NA;
      NULL;
    }
  
    eventKeybd <- function (key) {
      if (verbose) cat("eventKeybd( key = ", key, ")\n");
      if (key == "Up") {
        currentPhi <<- currentPhi - step;
        refresh.screen();
      } else if (key == "Down") {
        currentPhi <<- currentPhi + step;
        refresh.screen();
      } else if (key == "Left") {
        currentTheta <<- currentTheta - step;
        refresh.screen();
      } else if (key == "Right") {
        currentTheta <<- currentTheta + step;
        refresh.screen();
      };
      NULL;
    }
  
    refresh.screen();
    getGraphicsEvent("",
      onMouseDown = eventMouseDown,
      onMouseMove = eventMouseMove,
      onMouseUp = eventMouseUp,
      onKeybd = eventKeybd)
    }
  
  if (length(dim(x)) > 1) {
    run.Rotate3D(z = x, ...);
  } else {
    run.Rotate3D(x, ...);
  }
}

Rotate3D(outer(-10:10, -10:10, function(x, y) { r <- sqrt(x * x + y * y); 10 * sin(r) / r}), col = "lightblue");