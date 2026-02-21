use std::ffi::CStr;
use std::fs::File;
use std::io::Read;
use std::os::raw::c_char;
use std::ptr;

#[unsafe(no_mangle)]
pub extern "C" fn add(a: i32, b: i32) -> i32 {
    a + b
}

pub struct Xoshiro256StarStar {
    s: [u64; 4],
}

impl Xoshiro256StarStar {
    fn new(seed: u64) -> Self {
        let mut s = [0u64; 4];
        let mut x = seed;
        for i in 0..4 {
            x = x.wrapping_add(0x9e3779b97f4a7c15);
            let mut z = x;
            z = (z ^ (z >> 30)).wrapping_mul(0xbf58476d1ce4e5b9);
            z = (z ^ (z >> 27)).wrapping_mul(0x94d049bb133111eb);
            s[i] = z ^ (z >> 31);
        }
        Self { s }
    }

    fn next(&mut self) -> u64 {
        let result = rotl(self.s[1].wrapping_mul(5), 7).wrapping_mul(9);
        let t = self.s[1] << 17;

        self.s[2] ^= self.s[0];
        self.s[3] ^= self.s[1];
        self.s[1] ^= self.s[2];
        self.s[0] ^= self.s[3];

        self.s[2] ^= t;
        self.s[3] = rotl(self.s[3], 45);

        result
    }
}

fn rotl(x: u64, k: u32) -> u64 {
    (x << k) | (x >> (64 - k))
}

#[unsafe(no_mangle)]
pub extern "C" fn xoshiro256ss_new(seed: u64) -> *mut Xoshiro256StarStar {
    Box::into_raw(Box::new(Xoshiro256StarStar::new(seed)))
}

#[unsafe(no_mangle)]
pub extern "C" fn xoshiro256ss_next(ptr: *mut Xoshiro256StarStar) -> u64 {
    let rng = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };
    rng.next()
}

#[unsafe(no_mangle)]
pub extern "C" fn xoshiro256ss_free(ptr: *mut Xoshiro256StarStar) {
    if !ptr.is_null() {
        unsafe {
            drop(Box::from_raw(ptr));
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn calculate_md5_hash(path_ptr: *const c_char, out_hash: *mut u8) -> bool {
    if path_ptr.is_null() || out_hash.is_null() {
        return false;
    }

    let c_str = unsafe { CStr::from_ptr(path_ptr) };
    let path = match c_str.to_str() {
        Ok(s) => s,
        Err(_) => return false,
    };

    let mut file = match File::open(path) {
        Ok(f) => f,
        Err(_) => return false,
    };

    let mut buffer = Vec::new();
    if file.read_to_end(&mut buffer).is_err() {
        return false;
    }

    let digest = md5::compute(buffer);
    unsafe {
        ptr::copy_nonoverlapping(digest.as_ptr(), out_hash, 16);
    }

    true
}