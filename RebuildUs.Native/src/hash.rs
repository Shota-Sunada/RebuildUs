use std::ffi::CStr;
use std::fs::File;
use std::io::{BufReader, Read};
use std::os::raw::c_char;
use std::ptr;

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

    let file = match File::open(path) {
        Ok(f) => f,
        Err(_) => return false,
    };

    let mut reader = BufReader::new(file);
    let mut context = md5::Context::new();
    let mut buffer = [0u8; 8192];

    loop {
        match reader.read(&mut buffer) {
            Ok(0) => break,
            Ok(n) => context.consume(&buffer[..n]),
            Err(_) => return false,
        }
    }

    let digest = context.compute();
    unsafe {
        ptr::copy_nonoverlapping(digest.0.as_ptr(), out_hash, 16);
    }

    true
}