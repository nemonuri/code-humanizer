module Nemonuri.StratifiedNode

(*
만약 (t:eqtype) 이 아닌, (t:Type) 이라면,
'Failed to solve universe inequalities for inductives' 라는 오류가 발생한다.
저 't'에 stratified_node (...) 또는 stratified_node_list (...) 꼴 타입이 들어갈 수도 있어서인가.
만약 그럴 경우, 문제가 있나? 어떤 문제가 있지?
*)
type stratified_node (t:eqtype) : pos -> Type =
  | SNode : 
      #children_level:nat -> 
      children:(stratified_node_list t children_level) -> 
      value:t -> 
      stratified_node t (children_level + 1)
and stratified_node_list (t:eqtype) : nat -> Type =
  | SNil : 
      stratified_node_list t 0
  | SCons : 
      #hd_level:pos ->
      #tl_level:nat ->
      hd:(stratified_node t hd_level) ->
      tl:(stratified_node_list t tl_level) ->
      stratified_node_list t (if hd_level >= tl_level then hd_level else tl_level)


let stratified_node_func (t:eqtype) (t2:Type) =
  #lv:pos -> stratified_node t lv -> Tot t2

let stratified_node_predicate (t:eqtype) =
  stratified_node_func t bool

let refined_stratified_node_func
  (t:eqtype) (t2:Type) 
  (predicate:stratified_node_predicate t) =
  #lv:pos -> (sn:stratified_node t lv{predicate sn}) -> Tot t2

type stratified_node_with_level (t:eqtype) =
  | Snwl : #level:pos -> node:(stratified_node t level) -> stratified_node_with_level t

let get_stratified_node_with_level 
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Tot (stratified_node_with_level t)
  = Snwl sn
  