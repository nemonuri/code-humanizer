module Nemonuri.StratifiedNode.TypeTheory1

(*
compilation_unit = 
  | block*
  ;

block =
  | statement*
  ;

statement =
  | argument*
  ;

argument =
  | p0: block block*
  | p1: blockless_expression
  ;

blockless_expression =
  | (...)
  ;
*)

module L = FStar.List.Tot
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.NodeTheory
open Nemonuri.StratifiedNode.ListTheory

type argument_sum_kind =
  | Argument_s0
  | Argument_s1

type kind =
  | Compilation_unit
  | Block
  | Statement
  | Argument: argument_sum_kind -> kind
  | Blockless_expression

type data = { 
  kind: kind
}

let node (lv:pos) = (stratified_node data lv)
let node_list (mlv:nat) = (stratified_node_list data mlv)

let get_kind (#lv:pos) (sn:node lv)
  : Tot kind
  = sn.value.kind

let is_kind_blockless_expression (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_leaf sn) && (Blockless_expression? (get_kind sn))

let is_kind_argument (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_branch sn) && (Argument? (get_kind sn))

let get_argument_sum_kind (#lv:pos) (sn:node lv{ is_kind_argument sn })
  : Tot argument_sum_kind
  = let Argument v0 = (get_kind sn) in v0

let is_kind_argument_s0 (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_argument sn) && (Argument_s0? (get_argument_sum_kind sn)) //TODO: block block*

let is_kind_argument_s1 (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_argument sn) && 
    (Argument_s1? (get_argument_sum_kind sn)) &&
    (get_children_length sn = 1) &&
    (is_kind_blockless_expression (get_child_at sn 0))

(*
let is_kind_argument_s1 (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_branch sn) && (Blockless_expression? sn.value.kind)
*)