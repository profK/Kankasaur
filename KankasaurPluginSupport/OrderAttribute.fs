namespace KankasaurPluginSupport

open System

[<AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)>]
type OrderAttribute(order: int) =
    inherit Attribute()
    member this.Order = order