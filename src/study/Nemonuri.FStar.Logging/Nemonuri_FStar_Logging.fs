#light "off"
module Nemonuri_FStar_Logging

open System
open System.IO

let debug_print_string x = Printf.printf "%s" x; false