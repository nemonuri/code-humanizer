module Nemonuri.GreenNode

module L = FStar.List.Tot
module LT = Nemonuri.ListTheory

type green_node_schema (#t:Type) = {
  level:nat;
  value:t;
  children:(list (green_node_schema #t))
}

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