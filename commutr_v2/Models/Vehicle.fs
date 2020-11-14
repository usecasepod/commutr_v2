namespace CommutrV2.Models

open System

type Vehicle =
    { Id: Guid
      Make: string
      Model: string
      Year: int
      IsPrimary: bool }
