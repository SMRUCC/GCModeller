pub mod crandn;

const Mlng : i64 =  2147483648;
const namda : i64 = 314159269;
const C: i64 = 453806245;

#[no_mangle]
pub extern fn randn(m: i32, seed: i32) -> [f64] {
    let mut gauss: [f64, m];
    let mut tmp1: f64;
    let mut u : [f64];
    let mut k : i32;
    let stop: i32;

    if m % 2 == 0 {
        u = rand(m, seed);
        stop = m;
    } else {
        u = rand(m + 1, seed);
        stop = m - 1;
    }

    while k < stop {
        tmp1 = Math.Sqrt(-2 * Math.Log(1 - u(k)));
                gauss(k) = tmp1 * Math.Cos(2 * Math.PI * u(k + 1));
                gauss(k + 1) = tmp1 * Math.Sin(2 * Math.PI * u(k + 1));

                k = k + 2;
    }

    if m % 2 == 0 {
        tmp1 = Math.Sqrt(-2 * Math.Log(1 - u(m - 1)))
            gauss(m - 1) = tmp1 * Math.Cos(2 * Math.PI * u(m))
    }

    return gauss;
}

#[no_mangle]
pub extern fn rand(m: i32, seed: i32) -> [f64] {
    let mut goal : [f64, m];
    let mut x0 : i64 = seed as i64;
    let mut x1: i64;

    for k: usize in 0..m {
        x1 = (x0 * namda + C) % Mlng;
        goal[k] = x1 * 1.0 / Mlng;
        x0 = x1;
    }

    return goal;
}