namespace CommutrV2.Models
module VehicleId =
    type T = VehicleId of int
    let create id =
        VehicleId id
    let value (VehicleId id) = id