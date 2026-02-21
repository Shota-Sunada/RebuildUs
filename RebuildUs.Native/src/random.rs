use std::num::Wrapping;

pub struct Xoshiro256StarStar {
    s: [u64; 4],
}

impl Xoshiro256StarStar {
    pub fn new(seed: u64) -> Self {
        let mut s = [0u64; 4];
        let mut x = Wrapping(seed);
        for i in 0..4 {
            x += Wrapping(0x9e3779b97f4a7c15);
            let mut z = x;
            z = (z ^ (z >> 30)) * Wrapping(0xbf58476d1ce4e5b9);
            z = (z ^ (z >> 27)) * Wrapping(0x94d049bb133111eb);
            s[i] = (z ^ (z >> 31)).0;
        }
        Self { s }
    }

    pub fn next(&mut self) -> u64 {
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

pub struct Xoshiro256PlusPlus {
    s: [u64; 4],
}

impl Xoshiro256PlusPlus {
    pub fn new(seed: u64) -> Self {
        let mut s = [0u64; 4];
        let mut x = Wrapping(seed);
        for i in 0..4 {
            x += Wrapping(0x9e3779b97f4a7c15);
            let mut z = x;
            z = (z ^ (z >> 30)) * Wrapping(0xbf58476d1ce4e5b9);
            z = (z ^ (z >> 27)) * Wrapping(0x94d049bb133111eb);
            s[i] = (z ^ (z >> 31)).0;
        }
        Self { s }
    }

    pub fn next(&mut self) -> u64 {
        let result = rotl(self.s[0].wrapping_add(self.s[3]), 23).wrapping_add(self.s[0]);
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

pub struct Pcg64 {
    state: u64,
    inc: u64,
}

impl Pcg64 {
    pub fn new(seed: u64) -> Self {
        let mut pcg = Self {
            state: 0,
            inc: (seed << 1) | 1,
        };
        pcg.next();
        pcg.state = pcg.state.wrapping_add(seed);
        pcg.next();
        pcg
    }

    pub fn next(&mut self) -> u64 {
        let oldstate = self.state;
        self.state = oldstate.wrapping_mul(6364136223846793005).wrapping_add(self.inc);

        let word = ((oldstate >> ((oldstate >> 59) + 5)) ^ oldstate).wrapping_mul(12605985483714317049);
        (word >> 43) ^ word
    }
}

pub struct MersenneTwister {
    mt: [u32; 624],
    mti: usize,
}

impl MersenneTwister {
    pub fn new(seed: u32) -> Self {
        let mut mt = [0u32; 624];
        mt[0] = seed;
        for i in 1..624 {
            mt[i] = 1812433253u32
                .wrapping_mul(mt[i - 1] ^ (mt[i - 1] >> 30))
                .wrapping_add(i as u32);
        }
        Self { mt, mti: 624 }
    }

    pub fn next_u32(&mut self) -> u32 {
        if self.mti >= 624 {
            for k in 0..624 {
                let y = (self.mt[k] & 0x80000000) | (self.mt[(k + 1) % 624] & 0x7FFFFFFF);
                self.mt[k] = self.mt[(k + 397) % 624] ^ (y >> 1);
                if y % 2 != 0 {
                    self.mt[k] ^= 0x9908B0DF;
                }
            }
            self.mti = 0;
        }

        let mut y = self.mt[self.mti];
        self.mti += 1;

        y ^= y >> 11;
        y ^= (y << 7) & 0x9D2C5680;
        y ^= (y << 15) & 0xEFC60000;
        y ^= y >> 18;

        y
    }
}

fn rotl(x: u64, k: u32) -> u64 {
    (x << k) | (x >> (64 - k))
}

// FFI
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
pub extern "C" fn xoshiro256pp_new(seed: u64) -> *mut Xoshiro256PlusPlus {
    Box::into_raw(Box::new(Xoshiro256PlusPlus::new(seed)))
}

#[unsafe(no_mangle)]
pub extern "C" fn xoshiro256pp_next(ptr: *mut Xoshiro256PlusPlus) -> u64 {
    let rng = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };
    rng.next()
}

#[unsafe(no_mangle)]
pub extern "C" fn xoshiro256pp_free(ptr: *mut Xoshiro256PlusPlus) {
    if !ptr.is_null() {
        unsafe {
            drop(Box::from_raw(ptr));
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn pcg64_new(seed: u64) -> *mut Pcg64 {
    Box::into_raw(Box::new(Pcg64::new(seed)))
}

#[unsafe(no_mangle)]
pub extern "C" fn pcg64_next(ptr: *mut Pcg64) -> u64 {
    let rng = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };
    rng.next()
}

#[unsafe(no_mangle)]
pub extern "C" fn pcg64_free(ptr: *mut Pcg64) {
    if !ptr.is_null() {
        unsafe {
            drop(Box::from_raw(ptr));
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn mt19937_new(seed: u32) -> *mut MersenneTwister {
    Box::into_raw(Box::new(MersenneTwister::new(seed)))
}

#[unsafe(no_mangle)]
pub extern "C" fn mt19937_next(ptr: *mut MersenneTwister) -> u32 {
    let rng = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };
    rng.next_u32()
}

#[unsafe(no_mangle)]
pub extern "C" fn mt19937_free(ptr: *mut MersenneTwister) {
    if !ptr.is_null() {
        unsafe {
            drop(Box::from_raw(ptr));
        }
    }
}