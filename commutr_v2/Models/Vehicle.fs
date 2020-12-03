namespace CommutrV2.Models

module Vehicles =
    type Vehicle =
        { Id: int
          Make: string
          Model: string
          Year: int
          Odometer: decimal
          Notes: string
          IsPrimary: bool }
