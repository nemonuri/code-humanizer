module Nemonuri.GreenNode

module L = FStar.List.Tot
module LT = Nemonuri.ListTheory

(*
type green_node (#t:Type) = {
  value:t ;
  children:(list (green_node #t))
}
*)

type green_node_schema (#t:Type) = {
  level:nat;
  value:t;
  children:(list (green_node_schema #t))
}

let is_stratified (#t:Type) (gns:green_node_schema #t) : Tot bool =
  if gns.level = 0 then 
    L.isEmpty gns.children
  else
    (LT.has_type_unempty_list gns.children) &&
    L.for_all (fun (gns2:green_node_schema #t) -> gns2.level = gns.level - 1) gns.children

let has_type_green_node (#t:Type) (gns:green_node_schema #t) : Tot bool = is_stratified gns
let green_node (#t:Type) = gns:(green_node_schema #t){has_type_green_node gns}
  



(*
type leveled_green_node (#t:Type) : t -> nat -> Type =
  | Leveled_green_node_leaf : value:t -> leveled_green_node #t value 0
  | Leveled_green_node_node : level:nat -> value:t -> leveled_green_node #t value level
*)

(*
let get_prop_green_node_equality_comparer (#t:Type) (c:LT.prop_equality_comparer #t)
  : LT.prop_equality_comparer #(green_node #t)
  = fun (gn1:green_node #t) (gn2:green_node #t) ->
    (c gn1.value gn2.value) 
*)

let has_type_green_leaf (#t:Type) (gn:green_node #t) : bool =
  L.isEmpty gn.children
let green_leaf (#t:Type) = gn:(green_node #t){has_type_green_leaf gn}

(*
let has_type_green_node_child (#t:Type) (parent:green_node #t) (self:green_node #t) : bool =
  L.contains self parent.children
*)

(*
let has_type_finite_green_node (#t:Type) (gn:green_node #t) : prop =
*)

(*
let rec get_height (#t:Type) (gn:green_node #t)
  : pos
  = match gn.children with
  | [] -> 1
  | _ -> 
    let v1 = L.map (fun (v2:green_node #t) -> get_height v2) gn.children in
    let v3 = LT.max_for_unempty_list v1 LT.default_int_comparer in
    v3 + 1
*)