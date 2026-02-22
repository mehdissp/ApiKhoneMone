using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using System.Linq;

namespace JWTApi.Application.Helper;

    public class Ai
    {
        public static bool IsHouseRelated(string imagePath)
        {
            using var session = new InferenceSession("places365.onnx");

            Bitmap image = new Bitmap(imagePath);
            Bitmap resized = new Bitmap(image, new Size(224, 224));

            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = resized.GetPixel(x, y);
                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };

            using var results = session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            int topIndex = output
                .Select((value, index) => new { value, index })
                .OrderByDescending(x => x.value)
                .First().index;

            string predictedLabel = PlacesLabels.Labels[topIndex];

            string[] allowed =
            {
                "house", "apartment", "building",
                "bedroom", "living_room", "kitchen",
                "bathroom", "office", "corridor"
            };

            return allowed.Contains(predictedLabel);
        }
    public static class PlacesLabels
    {
        public static string[] Labels = new string[]
        {
        "airfield", "airplane_cabin", "airport_terminal", "alcove", "alley", "amphitheater",
        "amusement_arcade", "amusement_park", "apartment", "apartment_building", "apse",
        "aquarium", "aqueduct", "arcade", "arch", "archaelogical_excavation", "archway",
        "arena", "army_base", "art_gallery", "art_school", "art_studio", "artists_loft",
        "assembly_line", "atrium", "attic", "auditorium", "auto_factory", "auto_showroom",
        "badlands", "baggage_claim", "bakery", "balcony", "ball_pit", "ballroom", "bamboo_forest",
        "bank_vault", "banquet_hall", "bar", "barn", "barracks", "baseball_field", "basement",
        "basilica", "basketball_court", "bathroom", "batting_cage", "bayou", "bazaar",
        "beach", "beach_house", "beauty_salon", "bedchamber", "bedroom", "beer_garden",
        "beer_hall", "berth", "beverage_garden", "bistro", "boardwalk", "boat_deck",
        "boathouse", "bookstore", "booth", "botanical_garden", "bow_window", "bowling_alley",
        "boxing_ring", "bridge", "building_facade", "bullring", "burial_chamber", "bus_depot",
        "bus_interior", "butchers_shop", "butte", "cabana", "cabin", "cafeteria", "cage",
        "campsite", "campus", "canal", "canal_natural", "candy_store", "canyon", "carrousel",
        "casino", "castle", "catacomb", "cathedral", "cave", "cemetery", "chalet", "cheese_factory",
        "chemistry_lab", "chicken_coop", "church", "church_interior", "classroom", "clean_room",
        "cliff", "cloister", "closet", "clothing_store", "coast", "cockpit", "coffee_shop",
        "computer_room", "conference_center", "conference_room", "construction_site", "corn_field",
        "corral", "corridor", "cottage", "courthouse", "courtyard", "creek", "crevasse",
        "crosswalk", "cruise_ship", "cubicle", "dam", "delicatessen", "department_store",
        "desert", "desert_road", "diner", "dining_hall", "dining_room", "discotheque",
        "dorm_room", "downtown", "dressing_room", "driveway", "drugstore", "elevator",
        "elevator_shaft", "embassy", "enclosed_bridge", "entrance_hall", "escalator",
        "excavation", "fabric_store", "farm", "fastfood_restaurant", "field", "field_cultivated",
        "field_wild", "fire_escape", "fire_station", "fishpond", "flea_market", "florist_shop",
        "fog", "food_court", "football_field", "forest", "forest_path", "forest_road",
        "formal_garden", "fountain", "freeway", "fruit_and_vegetable_store", "fruit_stand",
        "funeral_home", "garage", "garbage_dump", "garden", "gas_station", "gazebo",
        "general_store", "gift_shop", "glacier", "golf_course", "greenhouse", "grotto",
        "gymnasium", "hangar", "harbor", "hardware_store", "hayfield", "heliport",
        "highway", "home_office", "home_theater", "homeless_shelter", "hospital",
        "hospital_room", "hot_spring", "hot_tub", "hotel", "hotel_room", "house",
        "hunting_lodge", "ice_cream_parlor", "ice_floe", "ice_skating_rink", "igloo",
        "industrial_area", "inn", "islet", "jacuzzi", "jail", "japanese_garden",
        "jewelry_shop", "junkyard", "kennel", "kindergarden", "kitchen", "lagoon",
        "lake", "landfill", "landing_deck", "laundromat", "lawn", "lecture_room",
        "library", "lighthouse", "living_room", "loading_dock", "lobby", "locker_room",
        "mansion", "manufactured_home", "market", "martial_arts_gym", "marsh", "massage_room",
        "maze", "meadow", "meat_packing", "medical_clinic", "medina", "mess_hall",
        "metro_station", "military_base", "minaret", "mine", "moat", "mobile_home",
        "monastery", "mosaic", "mosque", "motel", "mountain", "mountain_path",
        "mountain_snowy", "movie_theater", "museum", "music_studio", "natural_history_museum",
        "naval_base", "nursery", "oast_house", "ocean", "office", "office_building",
        "oil_refinery", "operating_room", "orchard", "orchestra_pit", "pagoda",
        "palace", "pantry", "park", "parking_garage", "parking_lot", "pasture",
        "patio", "pavilion", "pet_shop", "pharmacy", "phone_booth", "physics_laboratory",
        "picnic_area", "pier", "pizzeria", "playground", "playroom", "plaza",
        "pond", "poolroom", "port", "porch", "pottery", "power_plant", "prairie",
        "prayer_room", "preshcool", "promenade", "pub", "pulpit", "putting_green",
        "racecourse", "raceway", "raft", "railroad_track", "rainforest", "reception",
        "recreation_room", "repair_shop", "residential_neighborhood", "restaurant",
        "restaurant_kitchen", "restaurant_patio", "rice_paddy", "river", "rock_arch",
        "roof_garden", "room", "ruin", "runway", "safe", "science_museum", "shed",
        "shoe_shop", "shop", "shopping_mall", "shower", "ski_resort", "ski_slope",
        "sky", "skyscraper", "slum", "snack_bar", "soccer_field", "stables",
        "stadium", "stage", "staircase", "storage_room", "street", "subway_interior",
        "subway_station_platform", "supermarket", "sushi_bar", "swamp", "swimming_pool",
        "synagogue", "television_room", "television_studio", "temple", "tennis_court",
        "terrace", "throne_room", "ticket_booth", "tobacco_shop", "toyshop", "train_interior",
        "train_station_platform", "tree_farm", "tree_house", "trench", "tundra",
        "underwater", "utility_room", "valley", "vanity", "vegetable_garden",
        "veterinarians_office", "viaduct", "village", "vineyard", "volcano",
        "volleyball_court", "waiting_room", "water_park", "water_tower", "waterfall",
        "watering_hole", "wave", "wet_bar", "wheat_field", "wind_farm", "windmill",
        "wine_cellar", "wine_shop", "winter_landscape", "workshop", "yard",
        "youth_hostel", "zen_garden", "zoo"
        };
    }
}
    


