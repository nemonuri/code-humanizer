module Nemonuri.GreenNode

module L = FStar.List.Tot

type green_node (#t:Type) = {
  value:t ;
  children:(list (green_node #t))
}