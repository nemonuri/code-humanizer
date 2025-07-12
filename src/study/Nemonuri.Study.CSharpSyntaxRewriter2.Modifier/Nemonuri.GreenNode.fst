module Nemonuri.GreenNode

module L = FStar.List.Tot
module LT = Nemonuri.ListTheory

type green_node_schema (#t:Type) = {
  level:nat;
  value:t;
  children:(list (green_node_schema #t))
}

let is_valid_level_and_children_length (#t:Type) (gns:green_node_schema #t) : Tot bool =
  if L.isEmpty gns.children then
    gns.level = 0
  else
    gns.level > 0

let has_type_green_leaf_schema (#t:Type) (gns:green_node_schema #t) : Tot bool =
  (L.isEmpty gns.children) && 
  (gns.level = 0) &&
  (is_valid_level_and_children_length gns)
let green_leaf_schema (#t:Type) = gns:green_node_schema #t{has_type_green_leaf_schema gns}


let project_schema_level (#t:Type) (gns:green_node_schema #t) : Tot nat = gns.level

let convert_green_node_schemas_to_levels (#t:Type) (l:list (green_node_schema #t))
  : Tot (list nat)
  = L.map (project_schema_level #t) l

let get_max_level (#t:Type) (l:list (green_node_schema #t))
  : Tot nat
  = let nat_list = convert_green_node_schemas_to_levels l in
    LT.max nat_list LT.default_int_comparer 0

let has_type_green_branch_schema (#t:Type) (gns:green_node_schema #t) 
  : Tot bool
  = (not (L.isEmpty gns.children)) && 
    (gns.level = (get_max_level gns.children) + 1) &&
    (is_valid_level_and_children_length gns) &&
    (L.for_all is_valid_level_and_children_length gns.children)
let green_branch_schema (#t:Type) = gns:green_node_schema #t{has_type_green_branch_schema gns}

(*
let assert1 = assert (
  forall (#t:Type) (gbs:green_branch_schema #t{gbs.level = 1}).
    (L.for_all has_type_green_leaf_schema gbs.children)
)
*)

(*
let rec lemma1 (#t:Type) (gbs_hd:green_node_schema #t) (gbs_tl:(list (green_node_schema #t)))
  : Lemma (ensures gbs_hd.level > 0 || has_type_green_leaf_schema gbs_hd )
  = let children = gbs_hd.children in
    match (children, gbs_tl) with
    | ([], []) -> ()
    | ([], hd::tl) -> lemma1 hd tl
    | (hd::tl, []) -> lemma1 hd tl
    | (hd1::tl1, hd2::tl2) -> lemma1 hd1 (L.append tl1 gbs_tl)
*)

type top_level_strict_green_node (#t:Type) : nat -> Type =
  | Leaf : inner_leaf:green_leaf_schema #t -> top_level_strict_green_node #t inner_leaf.level
  | Branch : inner_branch:green_branch_schema #t -> top_level_strict_green_node #t ( (get_max_level inner_branch.children) + 1 )

type top_level_green_node (#t:Type) : nat -> Type =
  | TLGN : strict_level:nat -> 
         supplemented_level:nat ->
         inner_top_level_strict_green_node:top_level_strict_green_node #t strict_level ->
         top_level_green_node #t (strict_level + supplemented_level)

let has_type_green_leaf_schema_or_green_branch_schema (#t:Type) (gns:green_node_schema #t)
  : Tot bool
  = (has_type_green_leaf_schema gns) || (has_type_green_branch_schema gns)

let convert_to_top_level_strict_green_node 
  (#t:Type)
  (gns:green_node_schema #t{ 
    (has_type_green_leaf_schema_or_green_branch_schema gns)
  })
  : Tot (top_level_strict_green_node #t gns.level) =
  if has_type_green_branch_schema gns then
    Branch gns
  else
    Leaf gns

let loose_top_level_strict_green_node
  (#t:Type) (#strict_level:nat) (tlsgn:top_level_strict_green_node #t strict_level)
  (normal_level:nat{normal_level >= strict_level})
  : top_level_green_node #t normal_level
  = TLGN strict_level (normal_level - strict_level) tlsgn

let rec has_type_green_node (#t:Type) (#limit_level:nat) (tlgn:top_level_green_node #t limit_level)
  : Tot bool
  = 

(*
let rec has_type_strict_green_node (#t:Type) (#n:nat) (tlsgn:top_level_strict_green_node #t n)
  : Tot bool (decreases %[m;n])
  = match tlsgn with
  | Leaf _ -> true
  | Branch inner_branch -> has_type_strict_green_node_for_list inner_branch.children
and has_type_strict_green_node_for_list (#t:Type) (children:list (green_node_schema #t)) 
  : Tot bool (decreases children)
  = match children with
  | [] -> true
  | hd::tl ->
    (has_type_green_leaf_schema_or_green_branch_schema hd) &&
    (
      let tlsgn = convert_to_top_level_strict_green_node hd in
      has_type_strict_green_node tlsgn
    ) &&
    (has_type_strict_green_node_for_list tl)

let convert_strict_green_node_to_normal 
  (#t:Type) (#strict_level:nat) (sgn:strict_green_node #t strict_level)
  (normal_level:nat{normal_level >= strict_level})
  : green_node #t normal_level
  = GN strict_level (normal_level - strict_level) sgn

let project_level (#t:Type) (#n:nat) (gn:green_node #t n) : Tot nat =
  let GN _ _ sgn = gn in
  match sgn with
  | Leaf inner_leaf -> inner_leaf.level
  | Branch inner_branch -> inner_branch.level

let project_value (#t:Type) (#n:nat) (gn:green_node #t n) : Tot t =
  let GN _ _ sgn = gn in
  match sgn with
  | Leaf inner_leaf -> inner_leaf.value
  | Branch inner_branch -> inner_branch.value
*)

(*
let project_children (#t:Type) (#n:nat) (gn:green_node #t n) : Tot (list (green_node #t n)) =
  let GN _ _ sgn = gn in
  match sgn with
  | Leaf _ -> []
  | Branch inner_branch -> inner_branch.value
*)

(*
let assert1 = assert (
  forall (t:Type) (n:nat) (gn:green_node #t n).
    match gn with
    | Leaf v1 -> (v1.level = 0) /\ (v1.level = n)
    | Branch v2 -> (v2.level = n)
)
*)



(*
let rec is_stratified (#t:Type) (gns:green_node_schema #t) : Tot bool =
  if gns.level = 0 then 
    L.isEmpty gns.children
  else
    (LT.has_type_unempty_list gns.children) &&
    (
      let v1 = L.map (fun (v2:green_node_schema #t) -> v2.level) gns.children in
      let v3 = LT.max_for_unempty_list v1 LT.default_int_comparer in
      v3 = gns.level - 1
    ) &&
    (
      L.for_all (fun (v4:green_node_schema #t) -> is_stratified v4) gns.children
    )

let is_self_level_greater_then_children (#t:Type) (gns:green_node_schema #t) : Tot bool =
  if gns.level = 0 then 
    L.isEmpty gns.children
  else
    L.for_all (fun (gns2:green_node_schema #t) -> gns2.level < gns.level) gns.children
*)

(*
let rec is_weakly_stratified (#t:Type) (gns:green_node_schema #t) 
  : Tot bool (decreases gns.level) =
  (is_self_level_greater_then_children gns) &&
  (
    let children = gns.children in
    (L.for_all (fun (gns2:green_node_schema #t) -> is_self_level_greater_then_children gns2) children) &&
    (L.for_all (fun (gns2:green_node_schema #t) -> is_weakly_stratified gns2) children)
  )
*)

(*
let rec is_weakly_stratified (#t:Type) (gns:green_node_schema #t) : Tot bool =
  if gns.level = 0 then 
    L.isEmpty gns.children
  else
    let children = gns.children in
      if L.isEmpty children then 
        true
      else
        if L.for_all (fun (gns2:green_node_schema #t) -> gns2.level < gns.level) children then
          L.for_all (fun (gns2:green_node_schema #t) -> is_weakly_stratified gns2) children
        else
          false
*)

(*
let has_type_green_node (#t:Type) (gns:green_node_schema #t) : Tot bool = is_weakly_stratified gns
let green_node (#t:Type) = gns:(green_node_schema #t){has_type_green_node gns}

let has_type_green_leaf (#t:Type) (gn:green_node #t) : Tot bool =
  L.isEmpty gn.children
let green_leaf (#t:Type) = gn:(green_node #t){has_type_green_leaf gn}

let rec get_height (#t:Type) (gn:green_node #t)
  : Tot nat (decreases gn.level) =
  if has_type_green_leaf gn then 0
  else
    let v1 = L.map (fun (v2:green_node #t) -> get_height v2) gn.children in
    let v3 = LT.max_for_unempty_list v1 LT.default_int_comparer in
    v3 + 1
*)

// let assert_level_of_green_leaf_is_zero = assert ( forall (t:Type) (gl:green_leaf #t). gl.level = 0 )

(*
let assert_green_node_is_weak_green_node = assert ( 
  forall (t:Type) (gn:green_node #t). (has_type_weak_green_node gn)
)
*)