module Nemonuri.GreenNode

module L = FStar.List.Tot
module LT = Nemonuri.ListTheory

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
    (
      let v1 = L.map (fun (v2:green_node_schema #t) -> v2.level) gns.children in
      let v3 = LT.max_for_unempty_list v1 LT.default_int_comparer in
      v3 = gns.level - 1
    )

let has_type_green_node (#t:Type) (gns:green_node_schema #t) : Tot bool = is_stratified gns
let green_node (#t:Type) = gns:(green_node_schema #t){has_type_green_node gns}

let has_type_green_leaf (#t:Type) (gn:green_node #t) : Tot bool =
  L.isEmpty gn.children
let green_leaf (#t:Type) = gn:(green_node #t){has_type_green_leaf gn}

let assert_level_of_green_leaf_is_zero = assert ( forall (t:Type) (gl:green_leaf #t). gl.level = 0 )