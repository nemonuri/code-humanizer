module Nemonuri.StratifiedNodes.Decreasers

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module F = Nemonuri.StratifiedNodes.Factories
module I = Nemonuri.StratifiedNodes.Internals

//--- theory members ---

let get_decreaser_from_children #t (node:N.node t)
  : Tot (I.node_list_internal t (N.get_list_level (N.get_children node)))
  =
  (N.to_node_list_inverse (N.get_children node))


let prepend_child #t 
  (prepending:N.node t) (node:N.node t)
  : Pure (N.node t) True
    (ensures fun r -> 
      (get_decreaser_from_children node) << (get_decreaser_from_children r)
    )
  =
  let children = (N.get_children node) in
  let new_children = prepending::children in
  F.replace_children node new_children

let prepend_child_inverse #t
  (node:N.node t)
  : Pure ((N.node t) & (N.node t))
    (requires N.is_branch node)
    (ensures fun r -> (
      let (prepended, prev_node) = r in (
        (node = (prepend_child prepended prev_node)) /\
        ((get_decreaser_from_children prev_node) << (get_decreaser_from_children node))
      )
    ))
  =
  let prepended::prev_children = (N.get_children node) in
  let prev_node = F.replace_children node prev_children in
  (prepended, prev_node)

let get_prepended_node #t
  (node:N.node t{ N.is_branch node })
  =
  fst (prepend_child_inverse node)

let get_previous_node #t
  (node:N.node t{ N.is_branch node })
  =
  snd (prepend_child_inverse node)

//---|