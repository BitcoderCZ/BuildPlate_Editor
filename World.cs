using BuildPlate_Editor.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPlus;
using static BuildPlate_Editor.Util;

namespace BuildPlate_Editor
{
    public static class World
    {
        public static int number { get; private set; }

        // custom added reeds (was in .tga?)
        public static readonly ReadOnlyDictionary<string, string[]> texReplacements = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>()
        {
            { "brick_block", new []{ "brick" } },
            { "tnt", new []{ "tnt_side" } },
            { "lava", new []{ "lava_still" } },
            { "cobblestone_wall", new []{ "cobblestone" } },
            // log
            { "stripped_dark_oak_log", new []{ "stripped_dark_oak_log", "stripped_dark_oak_log_top" } },
            { "stripped_oak_log", new []{ "stripped_oak_log", "stripped_oak_log_top" } },
            { "stripped_spruce_log", new []{ "stripped_spruce_log", "stripped_spruce_log_top" } },
            // slabs
            { "double_wooden_slab", new []{ "planks" } },
            { "double_stone_slab", new []{ "stone" } },
            { "stone_slab", new []{ "stone" } },
            { "wooden_slab", new []{ "planks" } },
            { "double_stone_slab4", new []{ "stone_slab_side", "stone_slab_top", "stone_slab_top" } },
            // stairs
            { "stone_stairs", new []{ "stone" } },
            { "oak_stairs", new []{ "planks_oak" } },
            { "jungle_stairs", new []{ "planks_jungle" } },
            { "spruce_stairs", new []{ "planks_spruce" } },
            { "dark_oak_stairs", new []{ "planks_big_oak" } },
            { "birch_stairs", new []{ "planks_birch" } },
            { "stone_brick_stairs", new []{ "stonebrick" } },
            { "polished_andesite_stairs", new []{ "stone_andesite_smooth" } },
            { "polished_diorite_stairs", new []{ "stone_diorite_smooth" } },
            { "sandstone_stairs", new []{ "sandstone_normal" } },
            { "smooth_sandstone_stairs", new []{ "sandstone_top" } },
            { "polished_granite_stairs", new []{ "stone_granite_smooth" } },
            // terracotta
            { "yellow_glazed_terracotta", new []{ "glazed_terracotta_yellow" } },
            // buttons
            { "stone_button", new []{ "stone" } },
            // other
            { "carpet", new []{ "wool" } },
            { "grass_path", new []{ "grass_path_side", "grass_path_top", "dirt" } },
            { "red_sandstone", new []{ "red_sandstone_normal" } },
            { "packed_ice", new []{ "ice_packed" } },
            { "piston", new []{ "piston_top_normal" } },
            { "rainbow_wool", new []{ "rainbow_wool_a" } },
            { "rainbow_carpet", new []{ "rainbow_wool_a" } },
            { "hay_block", new []{ "hay_block_side" } },
            { "melon_block", new []{ "melon_side" } },
            { "pumpkin", new []{ "pumpkin_side" } },
            { "redstone_wire", new []{ "redstone_dust_line" } },
            { "quartz_block", new []{ "quartz_block_side" } }, // todo
            { "redstone_dust_dot", new []{ "redstone_dust_cross" } },
            { "golden_rail", new []{ "powered_rail" } },
            { "redstone_lamp", new []{ "redstone_lamp_off" } },
            { "lit_redstone_lamp", new []{ "redstone_lamp_on" } },
            { "powered_repeater", new []{ "repeater_on", /*"redstone_torch_on"*/ } },
            { "unpowered_repeater", new []{ "repeater_off", /*"redstone_torch_off"*/ } },
            { "dark_oak_fence_gate", new []{ "planks_big_oak" } },
            { "wheat", new []{ "wheat_stage_7" } },
            { "carrots", new []{ "carrots_stage_3" } },
            { "beetroot", new []{ "beetroots_stage_3" } },
            { "potatoes", new []{ "potatoes_stage_3" } },
            { "farmland", new []{ "farmland_dry" } },
            { "torch", new []{ "torch_on" } },
            { "mycelium", new []{ "mycelium_side", "mycelium_top", "dirt" } },
            { "sandstone", new []{ "sandstone_normal" } },
            { "red_mushroom", new []{ "mushroom_red" } },
            { "brown_mushroom", new []{ "mushroom_brown" } },
            { "tripWire", new []{ "trip_wire" } },
            { "pumpkin_stem", new []{ "pumpkin_stem_disconnected" } },
            { "melon_stem", new []{ "melon_stem_disconnected" } },
            { "tall_grass_bottom", new []{ "double_plant_grass_carried" } }, // bottom is .tga :(
            { "tallgrass", new []{ "double_plant_grass_carried" } },
            { "glass_pane", new []{ "glass" } },
            { "mossy_stone_brick_stairs", new []{ "stonebrick_mossy" } },
            { "mossy_cobblestone_stairs", new []{ "cobblestone_mossy" } },
            { "mossy_cobblestone", new []{ "cobblestone_mossy" } },
            { "concretePowder", new []{ "concrete_powder" } },
            { "mud", new []{ "mud_still" } },
            { "snow_layer", new []{ "snow" } },
            { "bone_block", new []{ "bone_block_top" } },
            { "redstone_torch", new []{ "redstone_torch_on" } },
            { "cauldron", new []{ "cauldron_side" } },
            { "unlit_redstone_torch", new []{ "redstone_torch_off" } },
            { "cocoa", new []{ "cocoa_stage_2" } },
            { "smooth_stone", new []{ "stone_slab_top" } }, // maybe not idk
            // terracotta
            { "white_glazed_terracotta", new []{ "glazed_terracotta_white" } },
            // flowers
            { "yellow_flower", new []{ "flower_dandelion" } },
            { "red_flower", new []{ "flower_rose" } },
            { "buttercup", new []{ "flower_buttercup" } },
            // fence
            { "fence", new []{ "planks" } },
            // gate
            { "birch_fence_gate", new []{ "planks_birch" } },
            { "jungle_fence_gate", new []{ "planks_jungle" } },
            { "fence_gate", new []{ "planks_oak" } },
            // preasure plate
            { "stone_pressure_plate", new []{ "stone" } },
            { "wooden_pressure_plate", new []{ "planks_oak" } },
            // button
            { "jungle_button", new []{ "planks_jungle" } },
            { "wooden_button", new []{ "planks_oak" } },
        });

        public static readonly Dictionary<string, Func<int, string[]>> specialTextureLoad = new Dictionary<string, Func<int, string[]>>() // texture => { data, return final texture}
        {
            { "planks", (int data) =>
                {
                    int woodType = data & 0b_0111;
                    switch (woodType) {
                        case 0:
                            return new [] { "planks_oak" } ;
                        case 1:
                            return new [] { "planks_spruce" } ;
                        case 2:
                            return new [] { "planks_birch" } ;
                        case 3:
                            return new [] { "planks_jungle" } ;
                        case 4:
                            return new [] { "planks_acacia" } ;
                        case 5:
                            return new [] { "planks_big_oak" } ;
                        default:
                            return new [] { "planks_oak" } ;
                    }
                }
            },
            { "wool", (int data) =>
                {
                    int woolType = data & 0b_1111;
                    switch (woolType) {
                        case 0:
                            return new [] { "wool_colored_white" } ;
                        case 1:
                            return new [] { "wool_colored_orange" } ;
                        case 2:
                            return new [] { "wool_colored_magenta" } ;
                        case 3:
                            return new [] { "wool_colored_light_blue" } ;
                        case 4:
                            return new [] { "wool_colored_yellow" } ;
                        case 5:
                            return new [] { "wool_colored_lime" } ;
                        case 6:
                            return new [] { "wool_colored_pink" } ;
                        case 7:
                            return new [] { "wool_colored_gray" } ;
                        case 8:
                            return new [] { "wool_colored_silver" } ;
                        case 9:
                            return new [] { "wool_colored_cyan" } ;
                        case 10:
                            return new [] { "wool_colored_purple" } ;
                        case 11:
                            return new [] { "wool_colored_blue" } ;
                        case 12:
                            return new [] { "wool_colored_brown" } ;
                        case 13:
                            return new [] { "wool_colored_green" } ;
                        case 14:
                            return new [] { "wool_colored_red" } ;
                        case 15:
                            return new [] { "wool_colored_black" } ;
                        default:
                            return new [] { "wool_colored_white" } ;
                    }
                }
            },
            { "log", (int data) =>
                {
                    int logType = data & 0b_0011;
                    switch (logType) {
                        case 0:
                            return new [] { "log_oak", "log_oak_top" } ;
                        case 1:
                            return new [] { "log_spruce", "log_spruce_top" } ;
                        case 2:
                            return new [] { "log_birch", "log_birch_top" } ;
                        case 3:
                            return new [] { "log_jungle", "log_jungle_top" } ;
                        default:
                            return new [] { "log_oak", "log_oak_top" } ;
                    }
                }
            },
            { "log2", (int data) =>
                {
                    int logType = data & 0b_0001;
                    switch (logType) {
                        case 0:
                            return new [] { "log_acacia", "log_acacia_top" } ;
                        case 1:
                            return new [] { "log_big_oak", "log_big_oak_top" } ;
                        default:
                            return new [] { "log_acacia", "log_acacia_top" } ;
                    }
                }
            },
            { "leaves", (int data) =>
                {
                    int leafType = data & 0b_0011;
                    string name;
                    switch (leafType) {
                        case 0:
                            name = "leaves_oak_opaque";
                            break;
                        case 1:
                            name =  "leaves_spruce_opaque";
                            break;
                        case 2:
                            name = "leaves_birch_opaque";
                            break;
                        case 3:
                            name = "leaves_jungle_opaque";
                            break;
                        default:
                            name =  "leaves_oak_opaque";
                            break;
                    }
                    string newName = name.Replace("_opaque", "");
                    // make green
                     SystemPlus.Utils.DirectBitmap db = SystemPlus.Utils.DirectBitmap.Load(textureBasePath + name + ".png", false);
                    byte darkest = 255;
                    for (int i = 0; i < db.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(db.Data[i]);
                        if (c.G < darkest)
                            darkest = c.G;
                    }
                    for (int i = 0; i < db.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(db.Data[i]);
                        db.Data[i] = System.Drawing.Color.FromArgb(c.G == darkest ? 0 : 255, 0, c.G, 0).ToArgb();
                    }
                    db.Bitmap.Save(textureBasePath + newName + ".png");
                    return new string[] { newName };
                }
            },
            { "leaves2", (int data) =>
                {
                    int leafType = data & 0b_0001;
                    string name;
                    switch (leafType) {
                        case 0:
                            name = "leaves_acacia_opaque";
                            break;
                        case 1:
                            name = "leaves_big_oak_opaque"; // dark oak
                            break;
                        default:
                            name = "leaves_acacia_opaque";
                            break;
                    }
                    string newName = name.Replace("_opaque", "");
                    // make green
                    SystemPlus.Utils.DirectBitmap db = SystemPlus.Utils.DirectBitmap.Load(textureBasePath + name + ".png", false);
                    byte darkest = 255;
                    for (int i = 0; i < db.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(db.Data[i]);
                        if (c.G < darkest)
                            darkest = c.G;
                    }
                    for (int i = 0; i < db.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(db.Data[i]);
                        db.Data[i] = System.Drawing.Color.FromArgb(c.G == darkest ? 0 : 255, 0, c.G, 0).ToArgb();
                    }
                    db.Bitmap.Save(textureBasePath + newName + ".png");
                    return new string[] { newName };
                }
            },
            { "concrete", (int data) =>
                {
                    int type = data & 0b_1111;
                    switch (type) {
                        case 0:
                            return new [] { "concrete_white" } ;
                        case 1:
                            return new [] { "concrete_orange" } ;
                        case 2:
                            return new [] { "concrete_magenta" } ;
                        case 3:
                            return new [] { "concrete_light_blue" } ;
                        case 4:
                            return new [] { "concrete_yellow" } ;
                        case 5:
                            return new [] { "concrete_lime" } ;
                        case 6:
                            return new [] { "concrete_pink" } ;
                        case 7:
                            return new [] { "concrete_gray" } ;
                        case 8:
                            return new [] { "concrete_silver" } ;
                        case 9:
                            return new [] { "concrete_cyan" } ;
                        case 10:
                            return new [] { "concrete_purple" } ;
                        case 11:
                            return new [] { "concrete_blue" } ;
                        case 12:
                            return new [] { "concrete_brown" } ;
                        case 13:
                            return new [] { "concrete_green" } ;
                        case 14:
                            return new [] { "concrete_red" } ;
                        case 15:
                            return new [] { "concrete_black" } ;
                        default:
                            return new [] { "concrete_white" } ;
                    }
                }
            },
            { "concrete_powder", (int data) =>
                {
                    int type = data & 0b_1111;
                    switch (type) {
                        case 0:
                            return new [] { "concrete_powder_white" } ;
                        case 1:
                            return new [] { "concrete_powder_orange" } ;
                        case 2:
                            return new [] { "concrete_powder_magenta" } ;
                        case 3:
                            return new [] { "concrete_powder_light_blue" } ;
                        case 4:
                            return new [] { "concrete_powder_yellow" } ;
                        case 5:
                            return new [] { "concrete_powder_lime" } ;
                        case 6:
                            return new [] { "concrete_powder_pink" } ;
                        case 7:
                            return new [] { "concrete_powder_gray" } ;
                        case 8:
                            return new [] { "concrete_powder_silver" } ;
                        case 9:
                            return new [] { "concrete_powder_cyan" } ;
                        case 10:
                            return new [] { "concrete_powder_purple" } ;
                        case 11:
                            return new [] { "concrete_powder_blue" } ;
                        case 12:
                            return new [] { "concrete_powder_brown" } ;
                        case 13:
                            return new [] { "concrete_powder_green" } ;
                        case 14:
                            return new [] { "concrete_powder_red" } ;
                        case 15:
                            return new [] { "concrete_powder_black" } ;
                        default:
                            return new [] { "concrete_powder_white" } ;
                    }
                }
            },
            { "dirt", (int data) =>
                {
                    int type = data & 0b_0001;
                    switch (type) {
                        case 0:
                            return new [] { "dirt" } ;
                        case 1:
                            return new [] { "coarse_dirt" } ;
                        default:
                            return new [] { "dirt" } ;
                    }
                }
            },
            { "double_plant", (int data) =>
                {
                    int plantType = data & 0b_0000;
                    switch (plantType) {
                        case 0:
                            return new [] { "double_plant_sunflower_front" } ;
                        case 1:
                            return new [] { "double_plant_syringa_top" } ;
                        case 2:
                            return new [] { "double_plant_grass_carried" } ;
                        case 3:
                            return new [] { "double_plant_fern_carried" } ;
                        case 4:
                            return new [] { "double_plant_rose_bottom" } ;
                        case 5:
                            return new [] { "double_plant_paeonia_top" } ;
                        default:
                            return new [] { "double_plant_sunflower_front" } ;
                    }
                }
            },
            { "glass", (int data) =>
                {
                    int type = data & 0b_1111;
                    switch (type) {
                        case 0:
                            return new [] { "glass_white" } ;
                        case 1:
                            return new [] { "glass_orange" } ;
                        case 2:
                            return new [] { "glass_magenta" } ;
                        case 3:
                            return new [] { "glass_light_blue" } ;
                        case 4:
                            return new [] { "glass_yellow" } ;
                        case 5:
                            return new [] { "glass_lime" } ;
                        case 6:
                            return new [] { "glass_pink" } ;
                        case 7:
                            return new [] { "glass_gray" } ;
                        case 8:
                            return new [] { "glass_silver" } ;
                        case 9:
                            return new [] { "glass_cyan" } ;
                        case 10:
                            return new [] { "glass_purple" } ;
                        case 11:
                            return new [] { "glass_blue" } ;
                        case 12:
                            return new [] { "glass_brown" } ;
                        case 13:
                            return new [] { "glass_green" } ;
                        case 14:
                            return new [] { "glass_red" } ;
                        case 15:
                            return new [] { "glass_black" } ;
                        default:
                            return new [] { "glass_white" } ;
                    }
                }
            },
            { "stained_hardened_clay", (int data) =>
                {
                    int type = data & 0b_0000;
                    switch (type) {
                         case 0:
                            return new [] { "hardened_clay_stained_white" } ;
                        case 1:
                            return new [] { "hardened_clay_stained_orange" } ;
                        case 2:
                            return new [] { "hardened_clay_stained_magenta" } ;
                        case 3:
                            return new [] { "hardened_clay_stained_light_blue" } ;
                        case 4:
                            return new [] { "hardened_clay_stained_yellow" } ;
                        case 5:
                            return new [] { "hardened_clay_stained_lime" } ;
                        case 6:
                            return new [] { "hardened_clay_stained_pink" } ;
                        case 7:
                            return new [] { "hardened_clay_stained_gray" } ;
                        case 8:
                            return new [] { "hardened_clay_stained_silver" } ;
                        case 9:
                            return new [] { "hardened_clay_stained_cyan" } ;
                        case 10:
                            return new [] { "hardened_clay_stained_purple" } ;
                        case 11:
                            return new [] { "hardened_clay_stained_blue" } ;
                        case 12:
                            return new [] { "hardened_clay_stained_brown" } ;
                        case 13:
                            return new [] { "hardened_clay_stained_green" } ;
                        case 14:
                            return new [] { "hardened_clay_stained_red" } ;
                        case 15:
                            return new [] { "hardened_clay_stained_black" } ;
                        default:
                            return new [] { "hardened_clay_stained_white" } ;
                    }
                }
            },
            { "rail", (int data) =>
                {
                    int direction = data & 0b_1111;
                    if (direction < 6)
                        return new [] { "rail_normal" } ;
                    else
                        return new [] { "rail_normal_turned" } ;
                }
            },
            { "powered_rail", (int data) =>
                {
                    int activated = data & 0b_1000 >> 3;
                    if (activated == 0)
                        return new [] { "rail_golden" } ;
                    else
                        return new [] { "rail_golden_powered" } ;
                }
            },
            { "detector_rail", (int data) =>
                {
                    int activated = data & 0b_1000 >> 3;
                    if (activated == 0)
                        return new [] { "rail_detector" } ;
                    else
                        return new [] { "rail_detector_powered" } ;
                }
            },
            { "stone_slab", (int data) =>
                {
                    int stone_slab_type = data & 0b_0111;
                    switch (stone_slab_type) {
                       case 0:
                            return new [] { "stone_slab_side", "stone_slab_top" };
                        case 1:
                            return new [] { "sandstone_normal", "sandstone_top", "sandstone_bottom" };
                        case 2:
                            return new [] { "planks_oak" };
                        case 3:
                            return new [] { "cobblestone" };
                        case 4:
                            return new [] { "brick" };
                        case 5:
                            return new [] { "stonebrick" };
                        case 6:
                            return new [] { "quartz_block_side", "quartz_block_top", "quartz_block_bottom" };
                        case 7:
                            return new [] { "nether_brick" };
                        default:
                            return new [] { "stone_slab_side", "stone_slab_top" };
                    }
                }
            },
            { "stone_slab2", (int data) =>
                {
                    int stone_slab_type_2 = data & 0b_0111;
                    switch (stone_slab_type_2) {
                        case 0:
                            return new [] { "red_sandstone_normal", "red_sandstone_top", "red_sandstone_bottom" };
                        case 1:
                            return new [] { "purpur_block" };
                        case 2:
                            return new [] { "prismarine_bricks" }; // supposed to be not brics, but texture to high, fuck it
                        case 3:
                            return new [] { "prismarine_dark" };
                        case 4:
                            return new [] { "prismarine_bricks" };
                        case 5:
                            return new [] { "cobblestone_mossy" };
                        case 6:
                            return new [] { "sandstone_top" };
                        case 7:
                            return new [] { "red_nether_brick" };
                        default:
                            return new [] { "red_sandstone_normal", "red_sandstone_top", "red_sandstone_bottom" };
                    }
                }
            },
            { "stone_slab3", (int data) =>
                {
                    int stone_slab_type_3 = data & 0b_0111;
                    switch (stone_slab_type_3) {
                      case 0:
                            return new [] { "end_bricks" };
                        case 1:
                            return new [] { "red_sandstone_top" };
                        case 2:
                            return new [] { "stone_andesite_smooth" };
                        case 3:
                            return new [] { "stone_andesite" };
                        case 4:
                            return new [] { "stone_diorite" };
                        case 5:
                            return new [] { "stone_diorite_smooth" };
                        case 6:
                            return new [] { "stone_granite" };
                        case 7:
                            return new [] { "stone_granite_smooth" };
                        default:
                            return new [] { "end_bricks" };
                    }
                }
            },
            { "stone_slab4", (int data) =>
                {
                    int stone_slab_type_4 = data & 0b_0111;
                    switch (stone_slab_type_4) {
                        case 0:
                            return new [] { "stonebrick_mossy" };
                        case 1:
                            return new [] { "quartz_block_side", "quartz_block_top" };
                        case 2:
                            return new [] { "stone" };
                        case 3:
                            return new [] { "sandstone_smooth", "sandstone_top" };
                        case 4:
                            return new [] { "red_sandstone_smooth", "red_sandstone_top" };
                        default:
                            return new [] { "stonebrick_mossy" };
                    }
                }
            },
            { "stained_glass", (int data) =>
                {
                    int type = data & 0b_1111;
                    switch (type) {
                        case 0:
                            return new [] { "glass_white" } ;
                        case 1:
                            return new [] { "glass_orange" } ;
                        case 2:
                            return new [] { "glass_magenta" } ;
                        case 3:
                            return new [] { "glass_light_blue" } ;
                        case 4:
                            return new [] { "glass_yellow" } ;
                        case 5:
                            return new [] { "glass_lime" } ;
                        case 6:
                            return new [] { "glass_pink" } ;
                        case 7:
                            return new [] { "glass_gray" } ;
                        case 8:
                            return new [] { "glass_silver" } ;
                        case 9:
                            return new [] { "glass_cyan" } ;
                        case 10:
                            return new [] { "glass_purple" } ;
                        case 11:
                            return new [] { "glass_blue" } ;
                        case 12:
                            return new [] { "glass_brown" } ;
                        case 13:
                            return new [] { "glass_green" } ;
                        case 14:
                            return new [] { "glass_red" };
                        case 15:
                            return new [] { "glass_black" };
                        default:
                            return new [] { "glass_white" };
                    }
                }
            },
            { "stained_glass_pane", (int data) =>
                {
                    int type = data & 0b_1111;
                    switch (type) {
                        case 0:
                            return new [] { "glass_white" } ;
                        case 1:
                            return new [] { "glass_orange" } ;
                        case 2:
                            return new [] { "glass_magenta" } ;
                        case 3:
                            return new [] { "glass_light_blue" } ;
                        case 4:
                            return new [] { "glass_yellow" } ;
                        case 5:
                            return new [] { "glass_lime" } ;
                        case 6:
                            return new [] { "glass_pink" } ;
                        case 7:
                            return new [] { "glass_gray" } ;
                        case 8:
                            return new [] { "glass_silver" } ;
                        case 9:
                            return new [] { "glass_cyan" } ;
                        case 10:
                            return new [] { "glass_purple" } ;
                        case 11:
                            return new [] { "glass_blue" } ;
                        case 12:
                            return new [] { "glass_brown" } ;
                        case 13:
                            return new [] { "glass_green" } ;
                        case 14:
                            return new [] { "glass_red" };
                        case 15:
                            return new [] { "glass_black" };
                        default:
                            return new [] { "glass_white" };
                    }
                }
            },
            { "vine", (int data) =>
                {
                    // make green
                    SystemPlus.Utils.DirectBitmap db = SystemPlus.Utils.DirectBitmap.Load(textureBasePath + "vine.png", false);
                    for (int i = 0; i < db.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(db.Data[i]);
                        db.Data[i] = System.Drawing.Color.FromArgb(c.A, 0, c.G, 0).ToArgb();
                    }
                    db.Bitmap.Save(textureBasePath + "vine.png");
                    return new string[] { "vine" };
                }
            },
            { "grass", (int data) =>
                {
                    // side and dirt need to combined
                    TargaImage img = new TargaImage(textureBasePath + "grass_side.tga");
                    SystemPlus.Utils.DirectBitmap side = SystemPlus.Utils.DirectBitmap.LoadFromBm(img.Image, false);
                    img.Dispose();
                    SystemPlus.Utils.DirectBitmap dirt = SystemPlus.Utils.DirectBitmap.Load(textureBasePath + "dirt.png", false);
                    for (int i = 0; i < side.Data.Length; i++) {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(side.Data[i]);
                        if (c.A == 0)
                            side.Data[i] = dirt.Data[i];
                        else
                            side[i] =  System.Drawing.Color.FromArgb(c.A, 0, c.G, 0).ToArgb();
                    }
                    side.Bitmap.Save(textureBasePath + "grass_side.png");
                    return new string[] { "grass_side.png", "grass_carried", "dirt" };
                }
            },
            { "water", (int data) =>
                {
                    // make green
                    SystemPlus.Utils.DirectBitmap db = SystemPlus.Utils.DirectBitmap.Load(textureBasePath + "water_still.png", false);
                    SystemPlus.Utils.DirectBitmap water = new SystemPlus.Utils.DirectBitmap(16, 16);

                    water.Write(0, db.Data, 0, water.Data.Length);

                    for (int i = 0; i < water.Data.Length; i++)
                    {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(water.Data[i]);
                        water.Data[i] = System.Drawing.Color.FromArgb(200, c.R, c.G, c.B).ToArgb();
                    }

                    water.Bitmap.Save(textureBasePath + "water.png");
                    return new string[] { "water" };
                }
            },
            { "cactus", (int data) => new [] { "cactus_side.tga", "cactus_top.tga", "cactus_bottom.tga" }
            },
            { "sapling", (int data) =>
                {
                    int type = data & 0b_0111;
                    switch (type) {
                        case 0:
                            return new [] { "sapling_oak" };
                        case 1:
                            return new [] { "sapling_spruce" };
                        case 2:
                            return new [] { "sapling_birch" };
                        case 3:
                            return new [] { "sapling_jungle" };
                        case 4:
                            return new [] { "sapling_acacia" };
                        case 5:
                            return new [] { "sapling_roofed_oak" }; // dark oak
                        default:
                            return new [] { "sapling_oak" };
                    }
                }
            },
            // doors
            { "iron_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_iron_lower" };
                        case 1:
                            return new [] { "door_iron_upper" };
                        default:
                            return new [] { "door_iron_lower" };
                    }
                }
            },
            { "jungle_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_jungle_lower" };
                        case 1:
                            return new [] { "door_jungle_upper" };
                        default:
                            return new [] { "door_jungle_lower" };
                    }
                }
            },
            { "wooden_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_wood_lower" };
                        case 1:
                            return new [] { "door_wood_upper" };
                        default:
                            return new [] { "door_wood_lower" };
                    }
                } // oak
            },
            { "birch_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_birch_lower" };
                        case 1:
                            return new [] { "door_birch_upper" };
                        default:
                            return new [] { "door_birch_lower" };
                    }
                }
            },
            { "spruce_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_spruce_lower" };
                        case 1:
                            return new [] { "door_spruce_upper" };
                        default:
                            return new [] { "door_spruce_lower" };
                    }
                }
            },
            { "dark_oak_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_dark_oak_lower" };
                        case 1:
                            return new [] { "door_dark_oak_upper" };
                        default:
                            return new [] { "door_dark_oak_lower" };
                    }
                }
            },
            { "acacia_door", (int data) =>
                {
                    int upper_block_bit = (data & 0b_1000) >> 3;
                    switch (upper_block_bit) {
                        case 0:
                            return new [] { "door_acacia_lower" };
                        case 1:
                            return new [] { "door_acacia_upper" };
                        default:
                            return new [] { "door_acacia_lower" };
                    }
                }
            },


            { "", (int data) =>
                {
                    int type = data & 0b_0000;
                    switch (type) {
                        case 0:
                            return new [] { "" } ;
                        case 1:
                            return new [] { "" } ;
                        case 2:
                            return new [] { "" } ;
                        case 3:
                            return new [] { "" } ;
                        case 4:
                            return new [] { "" } ;
                        case 5:
                            return new [] { "" } ;
                        case 6:
                            return new [] { "" } ;
                        case 7:
                            return new [] { "" } ;
                        case 8:
                            return new [] { "" } ;
                        case 9:
                            return new [] { "" } ;
                        case 10:
                            return new [] { "" } ;
                        case 11:
                            return new [] { "" } ;
                        case 12:
                            return new [] { "" } ;
                        case 13:
                            return new [] { "" } ;
                        case 14:
                            return new [] { "" };
                        case 15:
                            return new [] { "" };
                        default:
                            return new [] { "" };
                    }
                }
            },
            { "_", (int data) =>
                {
                    int type = data & 0b_0000;
                    switch (type) {
                        case 0:
                            return new [] { "" };
                        default:
                            return new [] { "" };
                    }
                }
            },
        };

        private static Dictionary<string, int> blockRenderersLookUp = new Dictionary<string, int>()
        {
            { "wheat", 5 },
            { "reeds", 5 },
            { "carrots", 5 },
            { "potatoes", 5 },
            { "beetroot", 5 },
            { "carpet", 6 },
            { "rainbow_carpet", 6 },
            { "waterlily", 6 }, // lilipad
            { "fence", 7 },
            { "pumpkin_stem", 8 },
            { "melon_stem", 8 },
            { "double_plant", 8 },
            { "tallgrass", 8 },
            { "red_mushroom", 8 },
            { "brown_mushroom", 8 },
            { "deadbush", 8 },
            { "buttercup", 8 },
            { "sapling", 8 },
            { "iron_bars", 9 },
            { "ladder", 10 },
            { "lantern", 11 },
            { "torch", 13 },
            { "redstone_torch", 13 },
            { "unlit_redstone_torch", 13 },
            { "snow_layer", 15 },
            { "powered_repeater", 16 },
            { "unpowered_repeater", 16 },
            { "vine", 19 },
            { "grass", 20 },
            { "grass_path", 20 },
            { "mycelium", 20 },
            { "double_stone_slab4", 20 },
            { "cactus", 21 },
            { "water", 22 },
            { "rail", 25 },
        };

        public static readonly bool[] RendererIsFullBlockLookUp = new bool[]
        {
            true, // normal
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            true, // log
            true, // stripped log
            false,
            true, // multi texture
            false,
            false,
            false,
            false,
            false, // rail
            false, // rail other
        };

        public delegate void RenderBlock(Vector3 pos, Vector3i cp/*chunk pos, pre multiplied*/, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles);

        // TODO stair other corner, Data meaning from: https://minecraft.fandom.com/wiki/Block_states
        public static Dictionary<int, RenderBlock> blockRenderers = new Dictionary<int, RenderBlock>()
        {
            { 0, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> verts, ref List<uint> tris) => // full
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size = new Vector3(1f, 1f, 1f);
                    uint tex = (uint)texA[0];

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)verts.Count;
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] * size - size / 2f, VoxelData.voxelUvs[0], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] * size - size / 2f, VoxelData.voxelUvs[1], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] * size - size / 2f, VoxelData.voxelUvs[2], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] * size - size / 2f, VoxelData.voxelUvs[3], tex));
                        tris.Add(firstVertIndex);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 3);
                    }
                }
            },
            { 1, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // slab
                {
                    bool upper = Convert.ToBoolean((data & 0b_1000) >> 3);
                    uint texSide = (uint)texA[0];
                    uint texTop = (uint)texA[texA.Length > 1 ? 1 : 0];
                    uint texBottom = (uint)texA[texA.Length > 2 ? 2 : (texA.Length > 1 ? 1 : 0)];

                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    Vector3[] verts;
                    Vector2[] uvs;
                    if (upper) {
                        verts = VoxelData.Slab.topVerts;
                        uvs = VoxelData.Slab.topUVs;
                    }
                    else {
                        verts = VoxelData.Slab.bottomVerts;
                        uvs = VoxelData.Slab.bottomUVs;
                    }

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 2) { // top
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texTop));
                        } else if (p == 3) { // bottom
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texBottom));
                        }
                        else {
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, uvs[0], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, uvs[1], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, uvs[2], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, uvs[3], texSide));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 2, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // stair's "slab"
                {
                    bool upper = Convert.ToBoolean((data & 0b_0100) >> 2);
                    uint texSide = (uint)texA[0];
                    uint texTop = (uint)texA[texA.Length > 1 ? 1 : 0];
                    uint texBottom = (uint)texA[texA.Length > 2 ? 2 : (texA.Length > 1 ? 1 : 0)];

                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    Vector3[] verts;
                    Vector2[] uvs;
                    if (upper) {
                        verts = VoxelData.Slab.topVerts;
                        uvs = VoxelData.Slab.topUVs;
                    }
                    else {
                        verts = VoxelData.Slab.bottomVerts;
                        uvs = VoxelData.Slab.bottomUVs;
                    }

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 2) { // top
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texTop));
                        } else if (p == 3) { // bottom
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texBottom));
                        }
                        else {
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, uvs[0], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, uvs[1], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, uvs[2], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, uvs[3], texSide));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }

                    blockRenderers[3](pos, cp, texA, data, ref vertices, ref triangles);
                }
            },
            { 3, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // stair's other part
                {
                    int rot = data & 0b_0011;
                    bool upper = Convert.ToBoolean((data & 0b_0100) >> 2);
                    Vector3 offset = new Vector3(0.25f, -0.25f, 0f);
                    Vector3 size = new Vector3(0.5f, 0.5f, 1f);

                    if (upper)
                        offset.Y = 0.25f;

                    if (rot == 1)
                        offset.X = -0.25f;
                    else if (rot > 1) {
                        offset.X = 0f;
                        size = new Vector3(1f, 0.5f, 0.5f);
                    }

                    if (rot == 2)
                        offset.Z = 0.25f;
                    else if (rot == 3)
                        offset.Z = -0.25f;

                    Vector3i iPos = new Vector3i((int)pos.X, (int)pos.Y, (int)pos.Z);
                    Vector3i[] offsets = new Vector3i[]
                    {
                        new Vector3i(1, 0, 0),
                        new Vector3i(-1, 0, 0),
                        new Vector3i(0, 0, 1),
                        new Vector3i(0, 0, -1),
                    };

                    GetBlockIndex(iPos + offsets[rot] + cp, out int subChunkIndex, out int blockIndex); // E W S N
                    if (subChunkIndex != -1 && chunks[subChunkIndex].renderers[blockIndex] == 2) {
                        int neightborData = GetBlockData(subChunkIndex, blockIndex);
                        int neightborRot = neightborData & 0b_0011;
                        if ((rot == 0 && neightborRot == 2) ||
                            (rot == 1 && neightborRot == 3) ||
                            (rot == 2 && neightborRot == 0) ||
                            (rot == 3 && neightborRot == 1)) { // left half gone
                            size = new Vector3(0.5f, 0.5f, 0.5f);
                            if (rot == 0)
                                offset.Z = 0.25f;
                            else if (rot == 1)
                                offset.Z = -0.25f;
                            else if (rot == 2)
                                offset.X = 0.25f;
                            else if (rot == 3)
                                offset.X = -0.25f;
                            CubeTex(texA[0], pos + offset, size, ref vertices, ref triangles, VoxelData.Stair.smallUvs);
                            return;
                        } else if ((rot == 0 && neightborRot == 3) ||
                            (rot == 1 && neightborRot == 2) ||
                            (rot == 2 && neightborRot == 1) ||
                            (rot == 3 && neightborRot == 0)) { // right half gone
                            size = new Vector3(0.5f, 0.5f, 0.5f);
                            if (rot == 0)
                                offset.Z = -0.25f;
                            else if (rot == 1)
                                offset.Z = 0.25f;
                            else if (rot == 2)
                                offset.X = -0.25f;
                            else if (rot == 3)
                                offset.X = 0.25f;
                            CubeTex(texA[0], pos + offset, size, ref vertices, ref triangles, VoxelData.Stair.smallUvs);
                            return;
                        }
                    }

                    uint texSide = (uint)texA[0];
                    uint texTop = (uint)texA[texA.Length > 1 ? 1 : 0];
                    uint texBottom = (uint)texA[texA.Length > 2 ? 2 : (texA.Length > 1 ? 1 : 0)];

                    offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    if (!upper)
                        offset.Y += 0.5f;

                    Vector3[] verts;
                    Vector2[] uvs = VoxelData.Stair.zUvs;

                    if (rot < 2) { // +x, -x, +z, -z
                        verts = VoxelData.Stair.xVerts;
                        uvs = VoxelData.Stair.xUvs;
                    }
                    else
                        verts = VoxelData.Stair.zVerts;

                    if (rot == 0)
                        offset.X += 0.5f;
                    else if (rot == 2)
                        offset.Z += 0.5f;

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p < 2 && rot < 2) { // z
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Stair.smallUvs[0], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Stair.smallUvs[1], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Stair.smallUvs[2], texTop));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Stair.smallUvs[3], texTop));
                        } else if (p > 3 && rot > 1) { // x
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Stair.smallUvs[0], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Stair.smallUvs[1], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Stair.smallUvs[2], texBottom));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Stair.smallUvs[3], texBottom));
                        }
                        else if (p == 2 || p == 3) { // top / bottom
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, uvs[0], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, uvs[1], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, uvs[2], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, uvs[3], texSide));
                        }
                        else {
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Stair.zUvs[0], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Stair.zUvs[1], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Stair.zUvs[2], texSide));
                            vertices.Add(new Vertex(pos + verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Stair.zUvs[3], texSide));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }

                    GetBlockIndex(iPos + (offsets[rot] * (-1)) + cp, out subChunkIndex, out blockIndex);


                }
            },
            { 4, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // trapdoor
                {
                    int dir = data & 0b_0011; // rotation
                    bool open = Convert.ToBoolean((data & 0b_1000) >> 3);
                    bool upsideDown = Convert.ToBoolean((data & 0b_0100) >> 2);

                    uint tex = (uint)texA[0];

                    if (open) {// -x, +x, -z, +z
                         for (int p = 0; p < 6; p++) {
                            Matrix3 transform;
                            Vector3 offset = -Vector3.One / 2f;
                            if (dir < 2){
                                transform = Matrix3.CreateRotationZ(1.5708f); // 90 degrees to radians
                                offset.X += 1f;
                                if (dir == 0)
                                    offset.X -= 0.8f;
                            }
                            else {
                                transform = Matrix3.CreateRotationX(-1.5708f); // -90 degrees to radians
                                offset.Z += 1f;
                                if (dir == 2)
                                    offset.Z -= 0.8f;
                            }
                            uint firstVertIndex = (uint)vertices.Count;
                            if (p == 2 || p == 3){
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] * transform + offset, VoxelData.voxelUvs[0], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] * transform + offset, VoxelData.voxelUvs[1], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] * transform + offset, VoxelData.voxelUvs[2], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] * transform + offset, VoxelData.voxelUvs[3], tex));
                            } else {
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] * transform + offset, VoxelData.Trapdoor.uVs[0], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] * transform + offset, VoxelData.Trapdoor.uVs[1], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] * transform + offset, VoxelData.Trapdoor.uVs[2], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] * transform + offset, VoxelData.Trapdoor.uVs[3], tex));
                            }
                            triangles.Add(firstVertIndex);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 3);
                        }
                    } else {
                         for (int p = 0; p < 6; p++) {
                            uint firstVertIndex = (uint)vertices.Count;
                            Vector3 offset = -Vector3.One / 2f;
                            if (upsideDown)
                                offset.Y += 0.8f;
                            if (p == 2 || p == 3){
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], tex));
                            } else {
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Trapdoor.uVs[0], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Trapdoor.uVs[1], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Trapdoor.uVs[2], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Trapdoor.uVs[3], tex));
                            }
                            triangles.Add(firstVertIndex);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 3);
                        }
                    }
                }
            },
            { 5, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // wheat...
                {
                    Vector3 offset1 = new Vector3(-0.3f, -0.1f, 0f);
                    Vector3 offset2 = new Vector3(0f, -0.1f, -0.3f);
                    Vector3 offset3 = new Vector3(0.3f, -0.1f, 0f);
                    Vector3 offset4 = new Vector3(0f, -0.1f, 0.3f);
                    Vector3 size1 = new Vector3(0.01f, 0.8f, 1f);
                    Vector3 size2 = new Vector3(1f, 0.8f, 0.01f);
                    CubeTex(tex[0], pos + offset1, size1, ref vertices, ref triangles);
                    CubeTex(tex[0], pos + offset2, size2, ref vertices, ref triangles);
                    CubeTex(tex[0], pos + offset3, size1, ref vertices, ref triangles);
                    CubeTex(tex[0], pos + offset4, size2, ref vertices, ref triangles);
                }
            },
            { 6,  (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // carpet
                {
                    Vector3 offset = new Vector3(0f, -0.45f, 0f);
                    Vector3 size = new Vector3(1f, 0.1f, 1f);
                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 7,  (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // fence
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size = new Vector3(0.4f, 1f, 0.4f);
                    Vector3 size2 = new Vector3(0.3f, 0.15f, 0.15f);
                    Vector3 size3 = new Vector3(0.15f, 0.15f, 0.3f);
                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);

                    Vector3i iPos = new Vector3i((int)pos.X, (int)pos.Y, (int)pos.Z);

                    int r1 = GetRenderer(iPos + new Vector3i(1, 0, 0) + cp);
                    int r2 = GetRenderer(iPos + new Vector3i(-1, 0, 0) + cp);
                    int r3 = GetRenderer(iPos + new Vector3i(0, 0, 1) + cp);
                    int r4 = GetRenderer(iPos + new Vector3i(0, 0, -1) + cp);
                    if (r1 == 7|| r1 == 12 || IsRendererFullBlock(r1)) {
                        CubeTex(tex[0], pos + new Vector3(0.35f, 0.15f, 0f), size2, ref vertices, ref triangles);
                        CubeTex(tex[0], pos + new Vector3(0.35f, -0.15f, 0f), size2, ref vertices, ref triangles);
                    }
                    if (r2 == 7|| r2 == 12|| IsRendererFullBlock(r2)) {
                        CubeTex(tex[0], pos + new Vector3(-0.35f, 0.15f, 0f), size2, ref vertices, ref triangles);
                        CubeTex(tex[0], pos + new Vector3(-0.35f, -0.15f, 0f), size2, ref vertices, ref triangles);
                    }
                    if (r3 == 7|| r3 == 12|| IsRendererFullBlock(r3)) {
                        CubeTex(tex[0], pos + new Vector3(0f, 0.15f, 0.35f), size3, ref vertices, ref triangles);
                        CubeTex(tex[0], pos + new Vector3(0f, -0.15f, 0.35f), size3, ref vertices, ref triangles);
                    }
                    if (r4 == 7|| r4 == 12|| IsRendererFullBlock(r4)) {
                        CubeTex(tex[0], pos + new Vector3(0f, 0.15f, -0.35f), size3, ref vertices, ref triangles);
                        CubeTex(tex[0], pos + new Vector3(0f, -0.15f, -0.35f), size3, ref vertices, ref triangles);
                    }
                }
            },
            { 8, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // double plant/flower
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size1 = new Vector3(0.01f, 0.8f, 1f);
                    Vector3 size2 = new Vector3(1f, 0.8f, 0.01f);
                    CubeTex(tex[0], pos + offset, size1, ref vertices, ref triangles);
                    CubeTex(tex[0], pos + offset, size2, ref vertices, ref triangles);
                }
            },
            { 9, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // glass pane/iron bars
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size = new Vector3(0.2f, 1f, 0.2f);
                    Vector3 size2 = new Vector3(0.4f, 1f, 0.2f);
                    Vector3 size3 = new Vector3(0.2f, 1f, 0.4f);
                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);

                    Vector3i iPos = new Vector3i((int)pos.X, (int)pos.Y, (int)pos.Z);

                    int r1 = GetRenderer(iPos + new Vector3i(1, 0, 0) + cp);
                    int r2 = GetRenderer(iPos + new Vector3i(-1, 0, 0) + cp);
                    int r3 = GetRenderer(iPos + new Vector3i(0, 0, 1) + cp);
                    int r4 = GetRenderer(iPos + new Vector3i(0, 0, -1) + cp);
                    if (r1 == 9 || IsRendererFullBlock(r1))
                        CubeTex(tex[0], pos + new Vector3(0.3f, 0f, 0f), size2, ref vertices, ref triangles);
                    if (r2 == 9|| IsRendererFullBlock(r2))
                        CubeTex(tex[0], pos + new Vector3(-0.3f, 0f, 0f), size2, ref vertices, ref triangles);
                    if (r3 == 9|| IsRendererFullBlock(r3))
                        CubeTex(tex[0], pos + new Vector3(0f, 0f, 0.3f), size3, ref vertices, ref triangles);
                    if (r4 == 9|| IsRendererFullBlock(r4))
                        CubeTex(tex[0], pos + new Vector3(0f, 0f, -0.3f), size3, ref vertices, ref triangles);
                }
            },
            { 10, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // ladder
                {
                    int facing = (data & 0b_0111) - 2; // 0, 1 unused (WTF)
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size;

                    if (facing > 1) // 2, 3 
                        size = new Vector3(0.2f, 1f, 1f);
                    else
                        size = new Vector3(1f, 1f, 0.2f);

                    switch (facing) { // might be wrong, thx wiki for good documentation
                        case 0:
                            offset = new Vector3(0f, 0f, 0.3f);
                            break;
                        case 1:
                            offset = new Vector3(0f, 0f, -0.3f);
                            break;
                        case 2:
                            offset = new Vector3(0.3f, 0f, 0f);
                            break;
                        case 3:
                            offset = new Vector3(-0.3f, 0f, 0f);
                            break;
                    }

                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 11, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // lantern
                {
                    Vector3 offset;
                    Vector3 size = new Vector3(0.3f, 0.4f, 0.3f);

                    bool hanging = Convert.ToBoolean(data & 0b_0001);

                    if (hanging) {
                        offset = new Vector3(0f, 0f, 0.1f);
                        Vector3 offset2 = new Vector3(0f, 0.4f, 0f);
                        Vector3 size2 = new Vector3(0.2f, 0.2f, 0.2f);
                        //DrawCube(pos + offset2, size2.X, size2.Y, size2.Z, BLACK); // "chain"
                    } else
                        offset = new Vector3(0f, -0.3f, 0f);

                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 12, (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // fence gate
                {
                    int facing = data & 0b_0011;
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size;

                    if (facing == 1 || facing == 3)
                        size = new Vector3(0.2f, 0.6f, 1f);
                    else
                        size = new Vector3(1f, 0.6f, 0.2f);

                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 13, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // torch
                {
                    uint tex = (uint)texA[0];
                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        Vector3 offset = -Vector3.One / 2f;
                        Matrix3 transform = Matrix3.Identity;
                        if (p == 2) { // top
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsTop[0], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsTop[1], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsTop[2], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsTop[3], tex));
                        } else if (p == 3) { // bottom
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsBottom[0], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsBottom[1], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsBottom[2], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsBottom[3], tex));
                        }
                        else {
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsSide[0], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsSide[1], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsSide[2], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsSide[3], tex));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 14,  (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // pressure_plate
                {
                    Vector3 offset = new Vector3(0f, -0.45f, 0f);
                    Vector3 size = new Vector3(0.8f, 0.1f, 0.8f);
                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 15,  (Vector3 pos, Vector3i cp, int[] tex, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // snow layer
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size = new Vector3(1f, 1f / 8f, 1f);
                    float step = 1 / 8f;
                    int height = data & 0b_0111;
                    size.Y = step * (float)(height + 1);
                    offset.Y = size.Y / -2f;
                    offset.Y = -(0.5f + offset.Y);

                    CubeTex(tex[0], pos + offset, size, ref vertices, ref triangles);
                }
            },
            { 16, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> verts, ref List<uint> tris) => // repeater
                {
                    int dir = data & 0b_0011;
                    Matrix3 mat = Matrix3.Identity;
                    Vector3 offset = Vector3.One / -2f;

                    if (dir == 0) {
                        mat =  Matrix3.Identity; // 0 degrees
                    }
                    else if (dir == 1) {
                        mat = Matrix3.CreateRotationY(1.5708f); // 90 degrees
                        offset.Z += 1f;
                    } else if (dir == 2) {
                        mat = Matrix3.CreateRotationY(3.14159f); // 180 degrees
                        offset.Z += 1f;
                    } else {
                        mat = Matrix3.CreateRotationY(4.71239f); // 270 degrees
                        offset.X += 1f;
                    }

                    uint tex = (uint)texA[0];
                    uint torchTex = tex;//(uint)texA[1];
                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)verts.Count;
                        if (p == 2 || p == 3) { // top/bottom
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 0]] * mat + offset, VoxelData.voxelUvs[0], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 1]] * mat + offset, VoxelData.voxelUvs[1], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 2]] * mat + offset, VoxelData.voxelUvs[2], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 3]] * mat + offset, VoxelData.voxelUvs[3], tex));
                        } else {
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 0]] * mat + offset, VoxelData.Repeater.sideUvs[0], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 1]] * mat + offset, VoxelData.Repeater.sideUvs[1], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 2]] * mat + offset, VoxelData.Repeater.sideUvs[2], tex));
                            verts.Add(new Vertex(pos + VoxelData.Repeater.verts[VoxelData.voxelTris[p, 3]] * mat + offset, VoxelData.Repeater.sideUvs[3], tex));
                        }
                        tris.Add(firstVertIndex);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 3);
                    }

                    return;
                    offset = -Vector3.One / 2f;
                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)verts.Count;
                        if (p == 2) { // top
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsTop[0], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsTop[1], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsTop[2], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsTop[3], torchTex));
                        } else if (p == 3) { // bottom
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsBottom[0], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsBottom[1], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsBottom[2], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsBottom[3], torchTex));
                        }
                        else {
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.Torch.uVsSide[0], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.Torch.uVsSide[1], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.Torch.uVsSide[2], torchTex));
                            verts.Add(new Vertex(pos + VoxelData.Torch.verts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.Torch.uVsSide[3], torchTex));
                        }
                        tris.Add(firstVertIndex);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 3);
                    }
                }
            },
            { 17, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // log
                {
                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    uint texSide = (uint)texA[0];
                    uint texTopBottom = (uint)texA[1];

                    int dir = (data & 0b_1100) >> 2; // 0 - y, 1 - x, 2 - z
                    Matrix3 mat = Matrix3.Identity;

                    if (dir == 1) {
                        mat = Matrix3.CreateRotationZ(1.5708f); // 90 degrees
                    } else if (dir == 2) {
                        mat = Matrix3.CreateRotationX(1.5708f); // 90 degrees
                    }

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 2 || p == 3) { // top/bottom
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] + offset) * mat, VoxelData.voxelUvs[0], texTopBottom));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] + offset) * mat, VoxelData.voxelUvs[1], texTopBottom));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] + offset) * mat, VoxelData.voxelUvs[2], texTopBottom));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] + offset) * mat, VoxelData.voxelUvs[3], texTopBottom));
                        } else {
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] + offset) * mat, VoxelData.voxelUvs[0], texSide));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] + offset) * mat, VoxelData.voxelUvs[1], texSide));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] + offset) * mat, VoxelData.voxelUvs[2], texSide));
                            vertices.Add(new Vertex(pos + (VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] + offset) * mat, VoxelData.voxelUvs[3], texSide));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 18, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // log stripped
                {
                    blockRenderers[17](pos, cp, texA, data << 2/*here axis is 0b0011, for normal logs it's 0b1100 so we need to shift it*/,
                        ref vertices, ref triangles);
                }
            },
            { 19, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // vine
                {
                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    uint tex = (uint)texA[0];

                    int dir = data & 0b_1111; // -z, -x, +z, +x

                    bool[] vineAt = new bool[4];
                    int mask = 0b_0001;
                    for (int i = 0; i < vineAt.Length; i++) // get every bit as bool
                    {
                        vineAt[i] = Convert.ToBoolean((dir & mask) >> i);
                        mask <<= i;
                    }

                    for (int i = 0; i < vineAt.Length; i++)
                    {
                        if (vineAt[i]) {
                            for (int p = 0; p < 2; p++)
                            {
                                uint firstVertIndex = (uint)vertices.Count;
                                vertices.Add(new Vertex(pos + VoxelData.Vine.verts[i, VoxelData.Vine.tris[p, 0]] + offset, VoxelData.voxelUvs[0], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Vine.verts[i,VoxelData.Vine.tris[p, 1]] + offset, VoxelData.voxelUvs[1], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Vine.verts[i,VoxelData.Vine.tris[p, 2]] + offset, VoxelData.voxelUvs[2], tex));
                                vertices.Add(new Vertex(pos + VoxelData.Vine.verts[i,VoxelData.Vine.tris[p, 3]] + offset, VoxelData.voxelUvs[3], tex));

                                triangles.Add(firstVertIndex);
                                triangles.Add(firstVertIndex + 1);
                                triangles.Add(firstVertIndex + 2);
                                triangles.Add(firstVertIndex + 2);
                                triangles.Add(firstVertIndex + 1);
                                triangles.Add(firstVertIndex + 3);
                            }
                        }
                    }
                }
            },
            { 20, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // multi texture
                {
                    bool upper = Convert.ToBoolean((data & 0b_1000) >> 3);
                    uint texSide = (uint)texA[0];
                    uint texTop = (uint)texA[texA.Length > 1 ? 1 : 0];
                    uint texBottom = (uint)texA[texA.Length > 2 ? 2 : (texA.Length > 1 ? 1 : 0)];

                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 2) { // top
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texTop));
                        } else if (p == 3) { // bottom
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texBottom));
                        }
                        else {
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] + offset, VoxelData.voxelUvs[0], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] + offset, VoxelData.voxelUvs[1], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] + offset, VoxelData.voxelUvs[2], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] + offset, VoxelData.voxelUvs[3], texSide));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 21, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // cactus
                {
                    bool upper = Convert.ToBoolean((data & 0b_1000) >> 3);
                    uint texSide = (uint)texA[0];
                    uint texTop = (uint)texA[1];
                    uint texBottom = (uint)texA[2];

                    Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 4) { // bottom
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 0]] + offset, VoxelData.Cactus.uvs[0], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 1]] + offset, VoxelData.Cactus.uvs[1], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 2]] + offset, VoxelData.Cactus.uvs[2], texBottom));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 3]] + offset, VoxelData.Cactus.uvs[3], texBottom));
                        } else if (p == 5) { // top
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 0]] + offset, VoxelData.Cactus.uvs[0], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 1]] + offset, VoxelData.Cactus.uvs[1], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 2]] + offset, VoxelData.Cactus.uvs[2], texTop));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 3]] + offset, VoxelData.Cactus.uvs[3], texTop));

                        } else {
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 0]] + offset, VoxelData.Cactus.uvs[0], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 1]] + offset, VoxelData.Cactus.uvs[1], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 2]] + offset, VoxelData.Cactus.uvs[2], texSide));
                            vertices.Add(new Vertex(pos + VoxelData.Cactus.verts[VoxelData.Cactus.tris[p, 3]] + offset, VoxelData.Cactus.uvs[3], texSide));
                        }

                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 22, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> verts, ref List<uint> tris) => // water
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    Vector3 size = new Vector3(1f, 1f, 1f);
                    uint tex = (uint)texA[0];

                    Vector3i iPos = new Vector3i((int)pos.X, (int)pos.Y, (int)pos.Z);

                    for (int p = 0; p < 6; p++) {
                        int rend = GetRenderer(iPos + cp + VoxelData.faceChecks[p]);
                        if (rend == 22)
                            continue;
                        uint firstVertIndex = (uint)verts.Count;
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]] * size - size / 2f, VoxelData.voxelUvs[0], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]] * size - size / 2f, VoxelData.voxelUvs[1], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]] * size - size / 2f, VoxelData.voxelUvs[2], tex));
                        verts.Add(new Vertex(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]] * size - size / 2f, VoxelData.voxelUvs[3], tex));
                        tris.Add(firstVertIndex);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 2);
                        tris.Add(firstVertIndex + 1);
                        tris.Add(firstVertIndex + 3);
                    }
                }
            },
            { 23, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // button
                {
                    int dir = data & 0b_0111; // +y, -y, +z, -z, +x, -x

                    uint tex = (uint)texA[0];

                    Matrix3 transform = Matrix3.Identity;
                    Vector3 offset = Vector3.Zero;
                    Vector3 defOff = -(new Vector3(VoxelData.Button.WidthH, VoxelData.Button.HeigthH, VoxelData.Button.DepthH));

                    if (dir < 2)
                        transform = Matrix3.CreateRotationX(1.5708f);
                    else if (dir > 3)
                        transform = Matrix3.CreateRotationY(1.5708f);

                    switch (dir)
                    {
                        case 0:
                            offset.Y += 0.5f - VoxelData.Button.HeigthH;
                            break;
                        case 1:
                            offset.Y -= 0.5f - VoxelData.Button.HeigthH;
                            break;
                        case 2:
                            offset.Z += 0.5f - VoxelData.Button.DepthH;
                            break;
                        case 3:
                            offset.Z -= 0.5f - VoxelData.Button.DepthH;
                            break;
                        case 4:
                            offset.X += 0.5f - VoxelData.Button.WidthH + VoxelData.Button.Depth;
                            break;
                        case 5:
                            offset.X -= 0.5f - VoxelData.Button.WidthH + VoxelData.Button.Depth;
                            break;
                        default:
                            break;
                    }


                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        int uvi = p / 2;
                        vertices.Add(new Vertex(pos + (VoxelData.Button.verts[VoxelData.voxelTris[p, 0]] + defOff) * transform + offset, VoxelData.Button.uvs[uvi, 0], tex));
                        vertices.Add(new Vertex(pos + (VoxelData.Button.verts[VoxelData.voxelTris[p, 1]] + defOff) * transform + offset, VoxelData.Button.uvs[uvi, 1], tex));
                        vertices.Add(new Vertex(pos + (VoxelData.Button.verts[VoxelData.voxelTris[p, 2]] + defOff) * transform + offset, VoxelData.Button.uvs[uvi, 2], tex));
                        vertices.Add(new Vertex(pos + (VoxelData.Button.verts[VoxelData.voxelTris[p, 3]] + defOff) * transform + offset, VoxelData.Button.uvs[uvi, 3], tex));

                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 24, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // door
                {
                    int dir = data & 0b_0011; // rotation
                    bool upper_block_bit = Convert.ToBoolean((data & 0b1000) >> 3);

                    if (upper_block_bit) {
                        Palette p = GetBlockPalette((Vector3i)pos + cp - Vector3i.UnitY);
                        if (p != Palette.NULL)
                            dir = p.data & 0b_0011;
                    }

                    uint tex = (uint)texA[0];

                    Matrix3 transform;
                    Vector3 offset = -Vector3.One / 2f;
                    // trapdoor: -x, +x, -z, +z, door: -x, +z, +x, -z
                    if (dir == 0 || dir == 2){
                        transform = Matrix3.CreateRotationZ(1.5708f); // 90 degrees to radians
                        offset.X += 1f;
                        if (dir == 2)
                            offset.X -= 0.8f;
                    }
                    else {
                        transform = Matrix3.CreateRotationX(-1.5708f); // -90 degrees to radians
                        offset.Z += 1f;
                        if (dir == 1)
                            offset.Z -= 0.8f;
                    }

                    for (int p = 0; p < 6; p++) {
                        uint firstVertIndex = (uint)vertices.Count;
                        if (p == 2 || p == 3){
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] * transform + offset, VoxelData.voxelUvs[0], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] * transform + offset, VoxelData.voxelUvs[1], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] * transform + offset, VoxelData.voxelUvs[2], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] * transform + offset, VoxelData.voxelUvs[3], tex));
                        } else {
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 0]] * transform + offset, VoxelData.Trapdoor.uVs[0], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 1]] * transform + offset, VoxelData.Trapdoor.uVs[1], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 2]] * transform + offset, VoxelData.Trapdoor.uVs[2], tex));
                            vertices.Add(new Vertex(pos + VoxelData.Trapdoor.verts[VoxelData.voxelTris[p, 3]] * transform + offset, VoxelData.Trapdoor.uVs[3], tex));
                        }
                        triangles.Add(firstVertIndex);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 2);
                        triangles.Add(firstVertIndex + 1);
                        triangles.Add(firstVertIndex + 3);
                    }
                }
            },
            { 25, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // rail
                {
                    uint tex = (uint)texA[0];

                    int shape = data & 0b_1111;

                    Matrix3 transform = Matrix3.Identity;
                    Vector3 offset = -Vector3.One / 2f;

                    if (shape < 2 || shape > 5) { // normal or curved
                        offset.Y -= VoxelData.Rail.Y - VoxelData.Rail.DefaultHeight;
                        if (shape == 1) {
                            transform = Matrix3.CreateRotationY(1.5708f); // 90 degrees
                        } else if (shape == 6) {
                            transform = Matrix3.CreateRotationY(-1.5708f); // -90 degrees
                        } else if (shape == 7) {
                            transform = Matrix3.CreateRotationY(3.14159f); // 180 degrees
                        } else if (shape == 8) {
                            transform = Matrix3.CreateRotationY(1.5708f); // 90 degrees
                        } 

                        for (int p = 0; p < 2; p++) {
                            uint firstVertIndex = (uint)vertices.Count;

                            vertices.Add(new Vertex(pos + (VoxelData.Rail.verts[VoxelData.Rail.tris[p, 0]] + offset) * transform, VoxelData.voxelUvs[0], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.verts[VoxelData.Rail.tris[p, 1]] + offset) * transform, VoxelData.voxelUvs[1], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.verts[VoxelData.Rail.tris[p, 2]] + offset) * transform, VoxelData.voxelUvs[2], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.verts[VoxelData.Rail.tris[p, 3]] + offset) * transform, VoxelData.voxelUvs[3], tex));

                            triangles.Add(firstVertIndex);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 3);
                        }
                    } else { // slope default - +x, +z
                         for (int p = 0; p < 2; p++) {
                            uint firstVertIndex = (uint)vertices.Count;

                            if (shape == 2) {
                                transform = Matrix3.CreateRotationY(1.5708f); // 90 degrees
                            } else if (shape == 3) {
                                transform = Matrix3.CreateRotationY(-1.5708f); // -90 degrees
                            } else if (shape == 4) {
                                transform = Matrix3.CreateRotationY(3.14159f); // 180 degrees
                            }

                            vertices.Add(new Vertex(pos + (VoxelData.Rail.vertsSlope[VoxelData.Rail.tris[p, 0]] + offset) * transform, VoxelData.voxelUvs[0], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.vertsSlope[VoxelData.Rail.tris[p, 1]] + offset) * transform, VoxelData.voxelUvs[1], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.vertsSlope[VoxelData.Rail.tris[p, 2]] + offset) * transform, VoxelData.voxelUvs[2], tex));
                            vertices.Add(new Vertex(pos + (VoxelData.Rail.vertsSlope[VoxelData.Rail.tris[p, 3]] + offset) * transform, VoxelData.voxelUvs[3], tex));

                            triangles.Add(firstVertIndex);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 2);
                            triangles.Add(firstVertIndex + 1);
                            triangles.Add(firstVertIndex + 3);
                        }
                    }
                }
            },
            { 26, (Vector3 pos, Vector3i cp, int[] texA, int data, ref List<Vertex> vertices, ref List<uint> triangles) => // rail other
                {
                    data &= 0b_0111; // filter out if is active
                    blockRenderers[25](pos, cp, texA, data, ref vertices, ref triangles);
                }
            },
        };

        public static BuildPlate plate;

        public static string textureBasePath;

        public static string BlockToPlace = string.Empty;

        private static string fileName;
        public static string targetFilePath;
        private static int fileRev;

        //===editor data
        private static int maxSubChunk;
        private static string selectedBlock;
        private static bool cursorActive;
        private static bool showConstraints;
        private static int layers;
        private static int slices;

        public static void UpdateChunkPalette(int subchunk)
        {
            Dictionary<int, string> blockNames = new Dictionary<int, string>();
            List<string[]> __textures = new List<string[]>();
            List<string> _textures = new List<string>();

            //Create the textures
            for (int paletteIndex = 0; paletteIndex < plate.sub_chunks[subchunk].block_palette.Count; paletteIndex++) {
                BuildPlate.PaletteBlock paletteBlock = plate.sub_chunks[subchunk].block_palette[paletteIndex];
                string[] blockName = new string[] { paletteBlock.name.Split(':')[1] };//gives us a clean texture name like dirt or grass_block

                blockNames.Add(paletteIndex, blockName[0]);

                if (texReplacements.ContainsKey(blockName[0]))
                    blockName = texReplacements[blockName[0]].Cloned();

                if (specialTextureLoad.ContainsKey(blockName[0]))
                    blockName = specialTextureLoad[blockName[0]].Invoke(paletteBlock.data).Cloned();

                for (int i = 0; i < blockName.Length; i++) {
                    if (blockName[i].Contains("."))
                        blockName[i] = textureBasePath + blockName[i];
                    else
                        blockName[i] = textureBasePath + blockName[i] + ".png";
                    _textures.Add(blockName[i]);
                }
                __textures.Add(blockName); // we assign the texture to this subchunks part of the texture dict
            }

            int[][] textures = new int[__textures.Count][];

            int c = 0;
            for (int i = 0; i < textures.Length; i++) {
                textures[i] = new int[__textures[i].Length];
                for (int j = 0; j < textures[i].Length; j++) {
                    textures[i][j] = c;
                    c++;
                }
            }

            int taid;
#if DEBUG
            taid = Texture.CreateTextureArray(_textures.ToArray(), TexFlip.Horizontal); // texture array id
#else
                try {
                    taid = Texture.CreateTextureArray(_textures.ToArray(), TexFlip.Horizontal); // texture array id
                } catch (Exception ex) {
                    Util.Exit(EXITCODE.World_ReLoad_TextureArray, ex);
                    return; // doesn't go here, just that vs is happy
                }
#endif

            Palette[] palette = new Palette[plate.sub_chunks[subchunk].block_palette.Count];
            for (int i = 0; i < palette.Length; i++) {
                int[] _tex = textures[i].Cloned();
                for (int j = 0; j < _tex.Length; j++)
                    _tex[j] = _tex[j] + 1;
                palette[i] = new Palette(plate.sub_chunks[subchunk].block_palette[i].name, plate.sub_chunks[subchunk].block_palette[i].data, _tex);
            }

            chunks[subchunk].palette = palette;
            chunks[subchunk].texId = taid;
        }

        public static uint GetBlock(Vector3i pos)
           => GetBlock(pos.X, pos.Y, pos.Z);
        public static uint GetBlock(int x, int y, int z)
        {
            GetBlockIndex(x, y, z, out int subChunkIndex, out int blockIndex);
            return GetBlock(subChunkIndex, blockIndex);
        }
        public static uint GetBlock(int subChunkIndex, int blockIndex)
            => chunks[subChunkIndex].blocks[blockIndex];

        public static void SetBlock(Vector3i pos, string blockName, int data = 0, bool compareData = false)
           => SetBlock(pos.X, pos.Y, pos.Z, blockName, data, compareData);
        public static void SetBlock(int x, int y, int z, string blockName, int data = 0, bool compareData = false)
        {
            GetBlockIndex(x, y, z, out int subChunkIndex, out int blockIndex);
            if (subChunkIndex == -1 || blockIndex == -1)
                return;
            SetBlock(subChunkIndex, blockIndex, blockName, data, compareData);
        }
        public static void SetBlock(int subChunkIndex, int blockIndex, string blockName, int data = 0, bool compareData = false)
        {
            SubChunk chunk = chunks[subChunkIndex];

            int pIndex = chunk.GetPaletteIndex(blockName, data, compareData);

            if (pIndex != -1)
                chunk.SetBlock(blockIndex, pIndex, GetRenderer(blockName), data, true);
            else {
                pIndex = chunk.AddNewPalette(blockName, data);
                chunk.SetBlock(blockIndex, pIndex, GetRenderer(blockName), data, true);
            }
        }

        public static int GetBlockData(Vector3i pos)
           => GetBlockData(pos.X, pos.Y, pos.Z);
        public static int GetBlockData(int x, int y, int z)
        {
            GetBlockIndex(x, y, z, out int subChunkIndex, out int blockIndex);
            return GetBlockData(subChunkIndex, blockIndex);
        }
        public static int GetBlockData(int subChunkIndex, int blockIndex)
        {
            try {
                return chunks[subChunkIndex].palette[chunks[subChunkIndex].blocks[blockIndex]].data;
            } catch { return 0; }
        }

        public static Palette GetBlockPalette(Vector3i pos)
           => GetBlockPalette(pos.X, pos.Y, pos.Z);
        public static Palette GetBlockPalette(int x, int y, int z)
        {
            GetBlockIndex(x, y, z, out int subChunkIndex, out int blockIndex);
            if (subChunkIndex < 0)
                return Palette.NULL;
            else
                return GetBlockPalette(subChunkIndex, blockIndex);
        }
        public static Palette GetBlockPalette(int subChunkIndex, int blockIndex)
            => chunks[subChunkIndex].palette[chunks[subChunkIndex].blocks[blockIndex]];

        public static int GetRenderer(Vector3i pos)
            => GetRenderer(pos.X, pos.Y, pos.Z);
        public static int GetRenderer(int x, int y, int z)
        {
            GetBlockIndex(x, y, z, out int subChunk, out int block);
            if (subChunk == -1)
                return -1;
            else
                return chunks[subChunk].renderers[block];
        }
        public static int GetRenderer(string blockName)
        {
            if (blockRenderersLookUp.ContainsKey(blockName))
                return blockRenderersLookUp[blockName];
            else if (blockName.Contains("rail") && blockName != "rail") // powered, detector, activator
                return 26;
            else if (blockName.Contains("button"))
                return 23;
            else if (blockName.Contains("stripped")) // stripped log
                return 18;
            else if (blockName.Contains("log"))
                return 17;
            else if (blockName.Contains("pressure_plate"))
                return 14;
            else if (blockName.Contains("gate"))
                return 12;
            else if (blockName.Contains("pane"))
                return 9;
            else if (blockName.Contains("flower"))
                return 8;
            else if (blockName.Contains("trapdoor"))
                return 4;
            else if (blockName.Contains("door"))
                return 24;
            else if (blockName.Contains("stairs"))
                return 2;
            else if (blockName.Contains("slab"))
                return 1;
            else if (blockName == "air")
                return -1;
            else if (blockName.Contains("constraint"))
                return -1;
            else
                return 0;
        }

        public static void GetBlockIndex(Vector3i pos, out int subChunkIndex, out int blockIndex)
            => GetBlockIndex(pos.X, pos.Y, pos.Z, out subChunkIndex, out blockIndex);
        public static void GetBlockIndex(int x, int y, int z, out int subChunkIndex, out int blockIndex)
        {
            for (int i = 0; i < plate.sub_chunks.Count; i++) {
                Vector3i pos = (Vector3i)plate.sub_chunks[i].position * 16;
                Vector3i posMax = pos + new Vector3i(16, 16, 16);
                if (x < pos.X || x > posMax.X ||
                   y < pos.Y || y > posMax.Y ||
                   z < pos.Z || z > posMax.Z)
                    continue;

                chunks[i].GetBlockIndex(x, y, z, out blockIndex); // stupid bad code PLS FIX

                if (blockIndex > -1 && blockIndex < 4096) {
                    subChunkIndex = i;
                    return;
                }
            }
            subChunkIndex = -1;
            blockIndex = -1;
            return;     
        }

        public static bool IsRendererFullBlock(int renderer)
        {
            if (renderer < 0 || renderer >= RendererIsFullBlockLookUp.Length)
                return false;
            else
                return RendererIsFullBlockLookUp[renderer];
        }

        public static void UpdateChunk(Vector3i pos)
        {
            for (int i = 0; i < chunks.Length; i++)
                if (chunks[i].pos == pos) {
                    chunks[i].Update();
                    return;
                }
        }

        public static SubChunk[] chunks;

        private static bool finishedInit = false;

        public static void Init()
        {
            plate = BuildPlate.Load(targetFilePath);
            
            string fileName = Path.GetFileNameWithoutExtension(targetFilePath); //get the filename with .plate
            int fileRev = 1; //used for saving so we dont save over a prior rev. useful when preparing multiple tests.

            Camera.position.Y = (float)Util.JsonDeserialize<JsonBuildPlate>(File.ReadAllText(targetFilePath)).offset.y;

            //===editor data
            maxSubChunk = plate.sub_chunks.Count - 1;
            selectedBlock = "portal";
            cursorActive = false;
            showConstraints = true;
            layers = 16;
            slices = 16;

            Dictionary<int, Dictionary<int, string>> blockNames = new Dictionary<int, Dictionary<int, string>>();

            chunks = new SubChunk[plate.sub_chunks.Count];

            Console.WriteLine("Loading...");
            for (int subchunk = 0; subchunk < plate.sub_chunks.Count; subchunk++) {
                List<string[]> __textures = new List<string[]>();
                List<string> _textures = new List<string>();

                blockNames.Add(subchunk, new Dictionary<int, string>());

                //Create the textures
                for (int paletteIndex = 0; paletteIndex < plate.sub_chunks[subchunk].block_palette.Count; paletteIndex++) {
                    BuildPlate.PaletteBlock paletteBlock = plate.sub_chunks[subchunk].block_palette[paletteIndex];
                    string[] blockName = new string[] { paletteBlock.name.Split(':')[1] };//gives us a clean texture name like dirt or grass_block

                    blockNames[subchunk].Add(paletteIndex, blockName[0]);

                    if (texReplacements.ContainsKey(blockName[0]))
                        blockName = texReplacements[blockName[0]].Cloned();

                    if (specialTextureLoad.ContainsKey(blockName[0]))
                        blockName = specialTextureLoad[blockName[0]].Invoke(paletteBlock.data).Cloned();

                    for (int i = 0; i < blockName.Length; i++) {
                        if (blockName[i].Contains("."))
                            blockName[i] = textureBasePath + blockName[i];
                        else
                            blockName[i] = textureBasePath + blockName[i] + ".png";
                        _textures.Add(blockName[i]);
                    }
                    __textures.Add(blockName); // we assign the texture to this subchunks part of the texture dict
                }

                int[][] textures = new int[__textures.Count][];

                int c = 0;
                for (int i = 0; i < textures.Length; i++) {
                    textures[i] = new int[__textures[i].Length];
                    for (int j = 0; j < textures[i].Length; j++) {
                        textures[i][j] = c;
                        c++;
                    }
                }

                int taid;
#if DEBUG
                taid = Texture.CreateTextureArray(_textures.ToArray(), TexFlip.Horizontal); // texture array id
#else
                try {
                    taid = Texture.CreateTextureArray(_textures.ToArray(), TexFlip.Horizontal); // texture array id
                } catch (Exception ex) {
                    Util.Exit(EXITCODE.World_Load_TextureArray, ex);
                    return; // doesn't go here, just that vs is happy
                }
#endif

                int[] renderers = new int[plate.sub_chunks[subchunk].blocks.Count];
                for (int block = 0; block < plate.sub_chunks[subchunk].blocks.Count; block++) {
                    string blockName = blockNames[subchunk][plate.sub_chunks[subchunk].blocks[block]];
                    renderers[block] = GetRenderer(blockName);
                }

                Palette[] palette = new Palette[plate.sub_chunks[subchunk].block_palette.Count];
                for (int i = 0; i < palette.Length; i++) {
                    int[] _tex = textures[i].Cloned();
                    for (int j = 0; j < _tex.Length; j++)
                        _tex[j] = _tex[j] + 1;
                    palette[i] = new Palette(plate.sub_chunks[subchunk].block_palette[i].name, plate.sub_chunks[subchunk].block_palette[i].data, _tex);
                    Console.WriteLine($"[{i}] Block: {palette[i].name}, Data: {palette[i].data}, " +
                        $"Textures: {textures[i].Length}");
                }

                chunks[subchunk] = new SubChunk(plate.sub_chunks[subchunk].position, 
                    plate.sub_chunks[subchunk].blocks.ForArray(i => (uint)i), renderers, palette, taid);
            }
        }

        public static void InitChunks()
        {
            Console.WriteLine("Initializing chunks");
            Parallel.For(0, chunks.Length, Util.DefaultParallelOptions, i => chunks[i].CreateMeshData());
        }

        public static void FinishInit()
        {
            // needs to run on main thread
            for (int i = 0; i < chunks.Length; i++)
                chunks[i].InitMesh();
            Console.WriteLine("DONE");
            finishedInit = true;
        }

        public static void Render(Shader s)
        {
            if (!finishedInit)
                FinishInit();

            for (int i = 0; i < chunks.Length; i++)
                chunks[i].Render(s);
        }
    }
}
