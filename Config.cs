﻿using System;
using System.Collections.Generic;
using Rocket.Core.Configuration;

namespace WreckingBall
{
	public class Config
	{
		public float DestructionInterval { get; set; } = 10;
		public uint DestructionsPerInterval { get; set; } = 10;

		// Clear vehicles from server ever `Vehicle Destruction interval` seconds
		public bool EnableVehicleCap { get; set; } = false;
		public uint VehicleDestructionInterval { get; set; } = 600;
		public ushort MaxVehiclesAllowed { get; set; } = 70;

		// Only destroys vehicle if buildables bellow `Min Element Count`
		public bool LowElementCountOnly { get; set; } = false;
		public ushort MinElementCount { get; set; } = 6;

		// Dont destroy vehicle if it has a sign with value `n`
		public bool KeepVehiclesWithSigns { get; set; } = true;
		public string VehicleSignFlag { get; set; } = "n";

		[ConfigArray (ElementName = "Catagory")]
		public List<Category> Categories { get; set; } = new List<Category>
		{
			new Category ('b', "Bed", ConsoleColor.DarkCyan),
			new Category ('t', "Trap", ConsoleColor.DarkYellow),
			new Category ('d', "Door", ConsoleColor.DarkMagenta),
			new Category ('c', "Container", ConsoleColor.Blue),
			new Category ('y', "Trophy Container", ConsoleColor.Blue),
			new Category ('l', "Ladder", ConsoleColor.Magenta),
			new Category ('w', "Wall", ConsoleColor.DarkMagenta),
			new Category ('f', "Floor", ConsoleColor.DarkMagenta),
			new Category ('p', "Pillar", ConsoleColor.DarkMagenta),
			new Category ('r', "Roof", ConsoleColor.DarkMagenta),
			new Category ('s', "Stair", ConsoleColor.DarkMagenta),
			new Category ('m', "Freeform", ConsoleColor.DarkMagenta),
			new Category ('n', "Sign", ConsoleColor.DarkBlue),
			new Category ('g', "Guard", ConsoleColor.DarkBlue),
			new Category ('o', "Protections", ConsoleColor.DarkBlue),
			new Category ('i', "Illumination", ConsoleColor.Yellow),
			new Category ('u', "Industrial", ConsoleColor.DarkYellow),
			new Category ('a', "Agriculture", ConsoleColor.Green),
			new Category ('A', "Animals", ConsoleColor.Cyan),
			new Category ('D', "Decorations", ConsoleColor.Yellow),
			new Category ('V', "Vehicles", ConsoleColor.DarkRed),
			new Category ('Z', "Zombies", ConsoleColor.DarkGreen),
			new Category ('!', "Uncategorized", ConsoleColor.White)
		};
		[ConfigArray (ElementName = "Element")]
		public List<Element> Elements { get; set; } = new List<Element>
		{
			new Element  (9998, 'Z'),
			new Element  (9999, 'V'),
			new Element  (288, 'b'),
			new Element  (289, 'b'),
			new Element  (290, 'b'),
			new Element  (291, 'b'),
			new Element  (292, 'b'),
			new Element  (293, 'b'),
			new Element  (294, 'b'),
			new Element  (295, 'b'),
			new Element  (1243, 'b'),
			new Element  (1309, 'b'),
			new Element  (1310, 'b'),
			new Element  (1311, 'b'),
			new Element  (1312, 'b'),
			new Element  (1313, 'b'),
			new Element  (1314, 'b'),
			new Element  (382, 't'),
			new Element  (383, 't'),
			new Element  (384, 't'),
			new Element  (385, 't'),
			new Element  (386, 't'),
			new Element  (1101, 't'),
			new Element  (1102, 't'),
			new Element  (1113, 't'),
			new Element  (1119, 't'),
			new Element  (1130, 't'),
			new Element  (1131, 't'),
			new Element  (1227, 't'),
			new Element  (1241, 't'),
			new Element  (1244, 'o'),
			new Element  (1372, 'o'),
			new Element  (1373, 'o'),
			new Element  (1393, 't'),
			new Element  (281, 'd'),
			new Element  (282, 'd'),
			new Element  (283, 'd'),
			new Element  (378, 'd'),
			new Element  (284, 'd'),
			new Element  (286, 'd'),
			new Element  (451, 'd'),
			new Element  (455, 'd'),
			new Element  (456, 'd'),
			new Element  (457, 'd'),
			new Element  (1235, 'd'),
			new Element  (1236, 'd'),
			new Element  (1237, 'd'),
			new Element  (1238, 'd'),
			new Element  (1329, 'd'),
			new Element  (1330, 'd'),
			new Element  (1331, 'd'),
			new Element  (1332, 'd'),
			new Element  (328, 'c'),
			new Element  (366, 'c'),
			new Element  (367, 'c'),
			new Element  (368, 'c'),
			new Element  (1374, 'c'),
			new Element  (1202, 'y'),
			new Element  (1203, 'y'),
			new Element  (1204, 'y'),
			new Element  (1205, 'y'),
			new Element  (1206, 'y'),
			new Element  (1207, 'y'),
			new Element  (1220, 'y'),
			new Element  (1221, 'y'),
			new Element  (1408, 'y'),
			new Element  (1409, 'y'),
			new Element  (1410, 'y'),
			new Element  (1411, 'y'),
			new Element  (1412, 'y'),
			new Element  (1413, 'y'),
			new Element  (325, 'l'),
			new Element  (326, 'l'),
			new Element  (327, 'l'),
			new Element  (379, 'l'),
			new Element  (33, 'w'),
			new Element  (57, 'w'),
			new Element  (58, 'w'),
			new Element  (371, 'w'),
			new Element  (1215, 'w'),
			new Element  (1414, 'w'),
			new Element  (1415, 'w'),
			new Element  (1416, 'w'),
			new Element  (1417, 'w'),
			new Element  (1418, 'w'),
			new Element  (32, 'w'),
			new Element  (49, 'w'),
			new Element  (50, 'w'),
			new Element  (370, 'w'),
			new Element  (1210, 'w'),
			new Element  (34, 'w'),
			new Element  (59, 'w'),
			new Element  (60, 'w'),
			new Element  (372, 'w'),
			new Element  (1216, 'w'),
			new Element  (450, 'w'),
			new Element  (452, 'w'),
			new Element  (453, 'w'),
			new Element  (454, 'w'),
			new Element  (1211, 'w'),
			new Element  (442, 'w'),
			new Element  (444, 'w'),
			new Element  (445, 'w'),
			new Element  (446, 'w'),
			new Element  (1214, 'w'),
			new Element  (31, 'f'),
			new Element  (51, 'f'),
			new Element  (52, 'f'),
			new Element  (369, 'f'),
			new Element  (1262, 'f'),
			new Element  (1263, 'f'),
			new Element  (1264, 'f'),
			new Element  (1265, 'f'),
			new Element  (36, 'p'),
			new Element  (53, 'p'),
			new Element  (54, 'p'),
			new Element  (374, 'p'),
			new Element  (1212, 'p'),
			new Element  (443, 'p'),
			new Element  (447, 'p'),
			new Element  (448, 'p'),
			new Element  (449, 'p'),
			new Element  (1213, 'p'),
			new Element  (35, 'r'),
			new Element  (55, 'r'),
			new Element  (56, 'r'),
			new Element  (373, 'r'),
			new Element  (1266, 'r'),
			new Element  (1267, 'r'),
			new Element  (1268, 'r'),
			new Element  (1269, 'r'),
			new Element  (319, 'r'),
			new Element  (321, 'r'),
			new Element  (320, 'r'),
			new Element  (376, 'r'),
			new Element  (316, 's'),
			new Element  (318, 's'),
			new Element  (317, 's'),
			new Element  (375, 's'),
			new Element  (322, 's'),
			new Element  (323, 's'),
			new Element  (324, 's'),
			new Element  (377, 's'),
			new Element  (1058, 'm'),
			new Element  (1059, 'm'),
			new Element  (1060, 'm'),
			new Element  (1061, 'm'),
			new Element  (1062, 'm'),
			new Element  (1063, 'm'),
			new Element  (1064, 'm'),
			new Element  (1065, 'm'),
			new Element  (1066, 'm'),
			new Element  (1067, 'm'),
			new Element  (1068, 'm'),
			new Element  (1069, 'm'),
			new Element  (1070, 'm'),
			new Element  (1071, 'm'),
			new Element  (1072, 'm'),
			new Element  (1073, 'm'),
			new Element  (1074, 'm'),
			new Element  (1075, 'm'),
			new Element  (1083, 'm'),
			new Element  (1084, 'm'),
			new Element  (1085, 'm'),
			new Element  (1086, 'm'),
			new Element  (1087, 'm'),
			new Element  (1088, 'm'),
			new Element  (1089, 'm'),
			new Element  (1090, 'm'),
			new Element  (1091, 'm'),
			new Element  (1092, 'm'),
			new Element  (1093, 'm'),
			new Element  (1094, 'm'),
			new Element  (1144, 'm'),
			new Element  (1145, 'm'),
			new Element  (1146, 'm'),
			new Element  (1147, 'm'),
			new Element  (1148, 'm'),
			new Element  (1149, 'm'),
			new Element  (1150, 'm'),
			new Element  (1151, 'm'),
			new Element  (1152, 'm'),
			new Element  (1153, 'm'),
			new Element  (1154, 'm'),
			new Element  (1155, 'm'),
			new Element  (1217, 'm'),
			new Element  (1218, 'm'),
			new Element  (1239, 'm'),
			new Element  (1396, 'm'),
			new Element  (1397, 'm'),
			new Element  (1095, 'n'),
			new Element  (1096, 'n'),
			new Element  (1097, 'n'),
			new Element  (1098, 'n'),
			new Element  (1231, 'n'),
			new Element  (1232, 'n'),
			new Element  (1233, 'n'),
			new Element  (1234, 'n'),
			new Element  (29, 'g'),
			new Element  (30, 'g'),
			new Element  (45, 'g'),
			new Element  (46, 'g'),
			new Element  (47, 'g'),
			new Element  (48, 'g'),
			new Element  (287, 'g'),
			new Element  (365, 'g'),
			new Element  (1223, 'g'),
			new Element  (1224, 'g'),
			new Element  (1225, 'g'),
			new Element  (1226, 'g'),
			new Element  (1297, 'g'),
			new Element  (1298, 'g'),
			new Element  (1299, 'g'),
			new Element  (1050, 'o'),
			new Element  (1158, 'o'),
			new Element  (1261, 'o'),
			new Element  (359, 'i'),
			new Element  (360, 'i'),
			new Element  (361, 'i'),
			new Element  (362, 'i'),
			new Element  (459, 'i'),
			new Element  (1049, 'i'),
			new Element  (1222, 'i'),
			new Element  (1255, 'i'),
			new Element  (1272, 'i'),
			new Element  (1273, 'i'),
			new Element  (1274, 'i'),
			new Element  (1275, 'i'),
			new Element  (1276, 'i'),
			new Element  (1277, 'i'),
			new Element  (458, 'u'),
			new Element  (1219, 'u'),
			new Element  (1208, 'u'),
			new Element  (1228, 'u'),
			new Element  (1229, 'u'),
			new Element  (1230, 'u'),
			new Element  (330, 'a'),
			new Element  (331, 'a'),
			new Element  (336, 'a'),
			new Element  (339, 'a'),
			new Element  (341, 'a'),
			new Element  (343, 'a'),
			new Element  (345, 'a'),
			new Element  (1045, 'a'),
			new Element  (1104, 'a'),
			new Element  (1105, 'a'),
			new Element  (1106, 'a'),
			new Element  (1107, 'a'),
			new Element  (1108, 'a'),
			new Element  (1109, 'a'),
			new Element  (1110, 'a'),
			new Element  (1345, 'a'),
			new Element  (1245, 'D'),
			new Element  (1246, 'D'),
			new Element  (1247, 'D'),
			new Element  (1248, 'D'),
			new Element  (1249, 'D'),
			new Element  (1250, 'D'),
			new Element  (1251, 'D'),
			new Element  (1252, 'D'),
			new Element  (1253, 'D'),
			new Element  (1254, 'D'),
			new Element  (1256, 'D'),
			new Element  (1257, 'D'),
			new Element  (1258, 'D'),
			new Element  (1259, 'D'),
			new Element  (1260, 'D'),
			new Element  (1278, 'D'),
			new Element  (1279, 'D'),
			new Element  (1280, 'D'),
			new Element  (1281, 'D'),
			new Element  (1282, 'D'),
			new Element  (1283, 'D'),
			new Element  (1284, 'D'),
			new Element  (1285, 'D'),
			new Element  (1286, 'D'),
			new Element  (1287, 'D'),
			new Element  (1288, 'D'),
			new Element  (1289, 'D'),
			new Element  (1290, 'D'),
			new Element  (1291, 'D'),
			new Element  (1292, 'D'),
			new Element  (1293, 'D'),
			new Element  (1294, 'D'),
			new Element  (1295, 'D'),
			new Element  (1296, 'D'),
			new Element  (1303, 'D'),
			new Element  (1304, 'D'),
			new Element  (1305, 'D'),
			new Element  (1306, 'D'),
			new Element  (1307, 'D'),
			new Element  (1308, 'D'),
			new Element  (1315, 'D'),
			new Element  (1316, 'D'),
			new Element  (1317, 'D'),
			new Element  (1318, 'D'),
			new Element  (1319, 'D'),
			new Element  (1320, 'D'),
			new Element  (1321, 'D'),
			new Element  (1322, 'D'),
			new Element  (1323, 'D'),
			new Element  (1324, 'D'),
			new Element  (1325, 'D'),
			new Element  (1326, 'D'),
			new Element  (1327, 'D'),
			new Element  (1328, 'D'),
			new Element  (1466, 'D'),
			new Element  (1500, 'm'),
			new Element  (1509, 'D'),
			new Element  (997, 'A')
		};
	}

	public class Category
	{
		public char Id { get; set; }
		public string Name { get; set; }
		public ConsoleColor Color { get; set; }

		public Category () { }
		internal Category (char id, string name, ConsoleColor color)
		{
			Id = id;
			Name = name;
			Color = color;
		}
	}

	public class Element
	{
		public ushort Id { get; set; }
		public char CategoryId { get; set; }

		public Element () { }
		internal Element (ushort ID, char Categoryid)
		{
			Id = ID;
			CategoryId = Categoryid;
		}
	}
}
