namespace CommutrV2.Models

open System

type Vehicle =
    { Id: int
      Make: string
      Model: string
      Year: int
      Odometer: decimal
      Notes: string
      IsPrimary: bool }
