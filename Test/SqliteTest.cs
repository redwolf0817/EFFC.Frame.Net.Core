using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB.Parameters;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class SqliteTest
    {
        public static void Test()
        {
            //var d = FrameDLRObject.CreateInstance(ComFunc.Base64DeCode(@"PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8bG9naW5SZXNwb25zZSBzZXNzaW9uSWQ9ImYyMGQ5OTA4LTJlMjQtNGUxYS04OWQxLWY0MzkyMjJjYzhhOCIvPgoKMjAxOC0wMy0xMiAxNToyODoxMCw4MzMgWzE0XTrmsZ/oi4/np7vliqjov5Tlm57nmoR4bWw6PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8cmVxdWVzdD4KICAgIDxoZWFkZXI+CiAgICAgICAgPHJlc3BvbnNldGltZT4yMDE4MDMxMjE1Mjg1MzwvcmVzcG9uc2V0aW1lPgogICAgICAgIDxzdXBwbGllcl9udW0+OTkxMDAwMTU8L3N1cHBsaWVyX251bT4KICAgIDwvaGVhZGVyPgogICAgPGJvZHk+CiAgICAgICAgPHJlc3VsdD4wPC9yZXN1bHQ+CiAgICAgICAgPHJlc3VsdHJlbWFyaz7miafooYzmiJDlip88L3Jlc3VsdHJlbWFyaz4KICAgICAgICA8b3JkZXJfbnVtYmVyPjQ8L29yZGVyX251bWJlcj4KICAgICAgICA8b3JkZXJfbGlzdHM+CiAgICAgICAgICAgIDxvcmRlcj4KICAgICAgICAgICAgICAgIDxvcmRlcl9ubz5CMjAxODAzMTIxNTIyNDk3MjA2MDc8L29yZGVyX25vPgogICAgICAgICAgICAgICAgPG9yZGVyX3N0YXR1cz48L29yZGVyX3N0YXR1cz4KICAgICAgICAgICAgICAgIDxvcmRlcl9yZWdpb24+MTI8L29yZGVyX3JlZ2lvbj4KICAgICAgICAgICAgICAgIDxyZWdpb25fbmFtZT7mt67lrok8L3JlZ2lvbl9uYW1lPgogICAgICAgICAgICAgICAgPGFyZWFfbnVtPjEyMTI8L2FyZWFfbnVtPgogICAgICAgICAgICAgICAgPGFyZWFfbmFtZT7mtp/msLQ8L2FyZWFfbmFtZT4KICAgICAgICAgICAgICAgIDxidWlsZF90aW1lPjIwMTgwMzEyMTUyMjQ5Nzk2PC9idWlsZF90aW1lPgogICAgICAgICAgICAgICAgPG9yZGVyX2RlbGl2X3RpbWU+MjAxODAzMTIxNTIzMDQ8L29yZGVyX2RlbGl2X3RpbWU+CiAgICAgICAgICAgICAgICA8YnVpbGRfbmFtZT7okovlubPlubM8L2J1aWxkX25hbWU+CiAgICAgICAgICAgICAgICA8YnVpbGRfdGVsPjE4OTAwMDAwMDAwPC9idWlsZF90ZWw+CiAgICAgICAgICAgICAgICA8ZGVwdF9jb2RlPjEyMTY0ODI4PC9kZXB0X2NvZGU+CiAgICAgICAgICAgICAgICA8ZGVwdF9uYW1lPua2n+awtOWVhuS4muW5v+WcuuaMh+WumuS4k+iQpeW6lzwvZGVwdF9uYW1lPgogICAgICAgICAgICAgICAgPGRpc3RfY29kZT4xMjEwNDIzMDwvZGlzdF9jb2RlPgogICAgICAgICAgICAgICAgPGRpc3RfbmFtZT7mm77ljY7vvIjnu4/plIDllYbvvIl0ZXN0PC9kaXN0X25hbWU+CiAgICAgICAgICAgICAgICA8b3JkZXJfbW9uZXk+MjA8L29yZGVyX21vbmV5PgogICAgICAgICAgICAgICAgPGRlbGl2X25hbWU+5reu5a6J5ZCI5L2c5Y6FMTwvZGVsaXZfbmFtZT4KICAgICAgICAgICAgICAgIDxkZWxpdl9tb2JpbGU+MTM5MDUxOTQ1ODE8L2RlbGl2X21vYmlsZT4KICAgICAgICAgICAgICAgIDxkZWxpdl9hZGRyZXNzPuS4reWkrui3rzQw5Y+3MTA25Y2V5YWDPC9kZWxpdl9hZGRyZXNzPgogICAgICAgICAgICAgICAgPGlzX21lZXRpbmc+MDwvaXNfbWVldGluZz4KICAgICAgICAgICAgICAgIDxtZWV0aW5nX25hbWU+PC9tZWV0aW5nX25hbWU+CiAgICAgICAgICAgICAgICA8cGF5X21vZGU+MTwvcGF5X21vZGU+CiAgICAgICAgICAgICAgICA8YWxsX3BheV9tb25leT4yMDwvYWxsX3BheV9tb25leT4KICAgICAgICAgICAgICAgIDxtdWx0aXBseV9wYXk+CiAgICAgICAgICAgICAgICAgICAgPHBheT4KICAgICAgICAgICAgICAgICAgICAgICAgPHBheV90aW1lPjIwMTgwMzEyMTUyMzA0PC9wYXlfdGltZT4KICAgICAgICAgICAgICAgICAgICAgICAgPHBheV9tb25leT4yMDwvcGF5X21vbmV5PgogICAgICAgICAgICAgICAgICAgICAgICA8cGF5X3R5cGU+RkxLPC9wYXlfdHlwZT4KICAgICAgICAgICAgICAgICAgICA8L3BheT4KICAgICAgICAgICAgICAgIDwvbXVsdGlwbHlfcGF5PgogICAgICAgICAgICAgICAgPGl0ZW1fbGlzdD4KICAgICAgICAgICAgICAgICAgICA8aXRlbT4KICAgICAgICAgICAgICAgICAgICAgICAgPGRldGFpbF9udW0+UzIwMTgwMzEyMTUyMjQ5NzIwNjA4PC9kZXRhaWxfbnVtPgogICAgICAgICAgICAgICAgICAgICAgICA8Z29vZHNfdHlwZT5TUExYX1NKWkQ8L2dvb2RzX3R5cGU+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19udW0+SlNZRC1ERFNDU1BQLUREU0NTWEg2LTAyPC9nb29kc19udW0+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19wcmljZT4xMDwvZ29vZHNfcHJpY2U+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19jb3VudD4yPC9nb29kc19jb3VudD4KICAgICAgICAgICAgICAgICAgICA8L2l0ZW0+CiAgICAgICAgICAgICAgICA8L2l0ZW1fbGlzdD4KICAgICAgICAgICAgICAgIDxnaWZ0X2xpc3QvPgogICAgICAgICAgICA8L29yZGVyPgogICAgICAgICAgICA8b3JkZXI+CiAgICAgICAgICAgICAgICA8b3JkZXJfbm8+QjIwMTgwMzEyMTUyNDQwNzIwNjE1PC9vcmRlcl9ubz4KICAgICAgICAgICAgICAgIDxvcmRlcl9zdGF0dXM+PC9vcmRlcl9zdGF0dXM+CiAgICAgICAgICAgICAgICA8b3JkZXJfcmVnaW9uPjEyPC9vcmRlcl9yZWdpb24+CiAgICAgICAgICAgICAgICA8cmVnaW9uX25hbWU+5reu5a6JPC9yZWdpb25fbmFtZT4KICAgICAgICAgICAgICAgIDxhcmVhX251bT4xMjEyPC9hcmVhX251bT4KICAgICAgICAgICAgICAgIDxhcmVhX25hbWU+5raf5rC0PC9hcmVhX25hbWU+CiAgICAgICAgICAgICAgICA8YnVpbGRfdGltZT4yMDE4MDMxMjE1MjQ0MDE5ODwvYnVpbGRfdGltZT4KICAgICAgICAgICAgICAgIDxvcmRlcl9kZWxpdl90aW1lPjIwMTgwMzEyMTUyNDU3PC9vcmRlcl9kZWxpdl90aW1lPgogICAgICAgICAgICAgICAgPGJ1aWxkX25hbWU+6JKL5bmz5bmzPC9idWlsZF9uYW1lPgogICAgICAgICAgICAgICAgPGJ1aWxkX3RlbD4xODkwMDAwMDAwMDwvYnVpbGRfdGVsPgogICAgICAgICAgICAgICAgPGRlcHRfY29kZT4xMjE2NDgyODwvZGVwdF9jb2RlPgogICAgICAgICAgICAgICAgPGRlcHRfbmFtZT7mtp/msLTllYbkuJrlub/lnLrmjIflrprkuJPokKXlupc8L2RlcHRfbmFtZT4KICAgICAgICAgICAgICAgIDxkaXN0X2NvZGU+MTIxMDQyMzA8L2Rpc3RfY29kZT4KICAgICAgICAgICAgICAgIDxkaXN0X25hbWU+5pu+5Y2O77yI57uP6ZSA5ZWG77yJdGVzdDwvZGlzdF9uYW1lPgogICAgICAgICAgICAgICAgPG9yZGVyX21vbmV5PjMwPC9vcmRlcl9tb25leT4KICAgICAgICAgICAgICAgIDxkZWxpdl9uYW1lPua3ruWuieWQiOS9nOWOhTE8L2RlbGl2X25hbWU+CiAgICAgICAgICAgICAgICA8ZGVsaXZfbW9iaWxlPjEzOTA1MTk0NTgxPC9kZWxpdl9tb2JpbGU+CiAgICAgICAgICAgICAgICA8ZGVsaXZfYWRkcmVzcz7kuK3lpK7ot680MOWPtzEwNuWNleWFgzwvZGVsaXZfYWRkcmVzcz4KICAgICAgICAgICAgICAgIDxpc19tZWV0aW5nPjA8L2lzX21lZXRpbmc+CiAgICAgICAgICAgICAgICA8bWVldGluZ19uYW1lPjwvbWVldGluZ19uYW1lPgogICAgICAgICAgICAgICAgPHBheV9tb2RlPjE8L3BheV9tb2RlPgogICAgICAgICAgICAgICAgPGFsbF9wYXlfbW9uZXk+MzA8L2FsbF9wYXlfbW9uZXk+CiAgICAgICAgICAgICAgICA8bXVsdGlwbHlfcGF5PgogICAgICAgICAgICAgICAgICAgIDxwYXk+CiAgICAgICAgICAgICAgICAgICAgICAgIDxwYXlfdGltZT4yMDE4MDMxMjE1MjQ1NzwvcGF5X3RpbWU+CiAgICAgICAgICAgICAgICAgICAgICAgIDxwYXlfbW9uZXk+MzA8L3BheV9tb25leT4KICAgICAgICAgICAgICAgICAgICAgICAgPHBheV90eXBlPkZMSzwvcGF5X3R5cGU+CiAgICAgICAgICAgICAgICAgICAgPC9wYXk+CiAgICAgICAgICAgICAgICA8L211bHRpcGx5X3BheT4KICAgICAgICAgICAgICAgIDxpdGVtX2xpc3Q+CiAgICAgICAgICAgICAgICAgICAgPGl0ZW0+CiAgICAgICAgICAgICAgICAgICAgICAgIDxkZXRhaWxfbnVtPlMyMDE4MDMxMjE1MjQ0MDcyMDYxNjwvZGV0YWlsX251bT4KICAgICAgICAgICAgICAgICAgICAgICAgPGdvb2RzX3R5cGU+U1BMWF9TSlpEPC9nb29kc190eXBlPgogICAgICAgICAgICAgICAgICAgICAgICA8Z29vZHNfbnVtPkpTWUQtRERTQ1NQUC1ERFNDU1hINi0wMjwvZ29vZHNfbnVtPgogICAgICAgICAgICAgICAgICAgICAgICA8Z29vZHNfcHJpY2U+MTA8L2dvb2RzX3ByaWNlPgogICAgICAgICAgICAgICAgICAgICAgICA8Z29vZHNfY291bnQ+MzwvZ29vZHNfY291bnQ+CiAgICAgICAgICAgICAgICAgICAgPC9pdGVtPgogICAgICAgICAgICAgICAgPC9pdGVtX2xpc3Q+CiAgICAgICAgICAgICAgICA8Z2lmdF9saXN0Lz4KICAgICAgICAgICAgPC9vcmRlcj4KICAgICAgICAgICAgPG9yZGVyPgogICAgICAgICAgICAgICAgPG9yZGVyX25vPkIyMDE4MDMxMjE1MjM1NzcyMDYxMTwvb3JkZXJfbm8+CiAgICAgICAgICAgICAgICA8b3JkZXJfc3RhdHVzPjwvb3JkZXJfc3RhdHVzPgogICAgICAgICAgICAgICAgPG9yZGVyX3JlZ2lvbj4xMjwvb3JkZXJfcmVnaW9uPgogICAgICAgICAgICAgICAgPHJlZ2lvbl9uYW1lPua3ruWuiTwvcmVnaW9uX25hbWU+CiAgICAgICAgICAgICAgICA8YXJlYV9udW0+MTIxMjwvYXJlYV9udW0+CiAgICAgICAgICAgICAgICA8YXJlYV9uYW1lPua2n+awtDwvYXJlYV9uYW1lPgogICAgICAgICAgICAgICAgPGJ1aWxkX3RpbWU+MjAxODAzMTIxNTIzNTc1ODg8L2J1aWxkX3RpbWU+CiAgICAgICAgICAgICAgICA8b3JkZXJfZGVsaXZfdGltZT4yMDE4MDMxMjE1MjQwNzwvb3JkZXJfZGVsaXZfdGltZT4KICAgICAgICAgICAgICAgIDxidWlsZF9uYW1lPuiSi+W5s+W5szwvYnVpbGRfbmFtZT4KICAgICAgICAgICAgICAgIDxidWlsZF90ZWw+MTg5MDAwMDAwMDA8L2J1aWxkX3RlbD4KICAgICAgICAgICAgICAgIDxkZXB0X2NvZGU+MTIxNjQ4Mjg8L2RlcHRfY29kZT4KICAgICAgICAgICAgICAgIDxkZXB0X25hbWU+5raf5rC05ZWG5Lia5bm/5Zy65oyH5a6a5LiT6JCl5bqXPC9kZXB0X25hbWU+CiAgICAgICAgICAgICAgICA8ZGlzdF9jb2RlPjEyMTA0MjMwPC9kaXN0X2NvZGU+CiAgICAgICAgICAgICAgICA8ZGlzdF9uYW1lPuabvuWNju+8iOe7j+mUgOWVhu+8iXRlc3Q8L2Rpc3RfbmFtZT4KICAgICAgICAgICAgICAgIDxvcmRlcl9tb25leT4zMDwvb3JkZXJfbW9uZXk+CiAgICAgICAgICAgICAgICA8ZGVsaXZfbmFtZT7mt67lronlkIjkvZzljoUxPC9kZWxpdl9uYW1lPgogICAgICAgICAgICAgICAgPGRlbGl2X21vYmlsZT4xMzkwNTE5NDU4MTwvZGVsaXZfbW9iaWxlPgogICAgICAgICAgICAgICAgPGRlbGl2X2FkZHJlc3M+5Lit5aSu6LevNDDlj7cxMDbljZXlhYM8L2RlbGl2X2FkZHJlc3M+CiAgICAgICAgICAgICAgICA8aXNfbWVldGluZz4wPC9pc19tZWV0aW5nPgogICAgICAgICAgICAgICAgPG1lZXRpbmdfbmFtZT48L21lZXRpbmdfbmFtZT4KICAgICAgICAgICAgICAgIDxwYXlfbW9kZT4xPC9wYXlfbW9kZT4KICAgICAgICAgICAgICAgIDxhbGxfcGF5X21vbmV5PjMwPC9hbGxfcGF5X21vbmV5PgogICAgICAgICAgICAgICAgPG11bHRpcGx5X3BheT4KICAgICAgICAgICAgICAgICAgICA8cGF5PgogICAgICAgICAgICAgICAgICAgICAgICA8cGF5X3RpbWU+MjAxODAzMTIxNTI0MDc8L3BheV90aW1lPgogICAgICAgICAgICAgICAgICAgICAgICA8cGF5X21vbmV5PjMwPC9wYXlfbW9uZXk+CiAgICAgICAgICAgICAgICAgICAgICAgIDxwYXlfdHlwZT5KQks8L3BheV90eXBlPgogICAgICAgICAgICAgICAgICAgIDwvcGF5PgogICAgICAgICAgICAgICAgPC9tdWx0aXBseV9wYXk+CiAgICAgICAgICAgICAgICA8aXRlbV9saXN0PgogICAgICAgICAgICAgICAgICAgIDxpdGVtPgogICAgICAgICAgICAgICAgICAgICAgICA8ZGV0YWlsX251bT5TMjAxODAzMTIxNTIzNTc3MjA2MTI8L2RldGFpbF9udW0+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc190eXBlPlNQTFhfU0paRDwvZ29vZHNfdHlwZT4KICAgICAgICAgICAgICAgICAgICAgICAgPGdvb2RzX251bT5KU1lELUREU0NTUFAtRERTQ1NYSDYtMDI8L2dvb2RzX251bT4KICAgICAgICAgICAgICAgICAgICAgICAgPGdvb2RzX3ByaWNlPjEwPC9nb29kc19wcmljZT4KICAgICAgICAgICAgICAgICAgICAgICAgPGdvb2RzX2NvdW50PjM8L2dvb2RzX2NvdW50PgogICAgICAgICAgICAgICAgICAgIDwvaXRlbT4KICAgICAgICAgICAgICAgIDwvaXRlbV9saXN0PgogICAgICAgICAgICAgICAgPGdpZnRfbGlzdC8+CiAgICAgICAgICAgIDwvb3JkZXI+CiAgICAgICAgICAgIDxvcmRlcj4KICAgICAgICAgICAgICAgIDxvcmRlcl9ubz5CMjAxODAzMTIxNTEzMjg3MjA2MDM8L29yZGVyX25vPgogICAgICAgICAgICAgICAgPG9yZGVyX3N0YXR1cz48L29yZGVyX3N0YXR1cz4KICAgICAgICAgICAgICAgIDxvcmRlcl9yZWdpb24+MTI8L29yZGVyX3JlZ2lvbj4KICAgICAgICAgICAgICAgIDxyZWdpb25fbmFtZT7mt67lrok8L3JlZ2lvbl9uYW1lPgogICAgICAgICAgICAgICAgPGFyZWFfbnVtPjEyMTI8L2FyZWFfbnVtPgogICAgICAgICAgICAgICAgPGFyZWFfbmFtZT7mtp/msLQ8L2FyZWFfbmFtZT4KICAgICAgICAgICAgICAgIDxidWlsZF90aW1lPjIwMTgwMzEyMTUxMzI4MjY3PC9idWlsZF90aW1lPgogICAgICAgICAgICAgICAgPG9yZGVyX2RlbGl2X3RpbWU+MjAxODAzMTIxNTIzMTg8L29yZGVyX2RlbGl2X3RpbWU+CiAgICAgICAgICAgICAgICA8YnVpbGRfbmFtZT7okovlubPlubM8L2J1aWxkX25hbWU+CiAgICAgICAgICAgICAgICA8YnVpbGRfdGVsPjE4OTAwMDAwMDAwPC9idWlsZF90ZWw+CiAgICAgICAgICAgICAgICA8ZGVwdF9jb2RlPjEyMTY0ODI4PC9kZXB0X2NvZGU+CiAgICAgICAgICAgICAgICA8ZGVwdF9uYW1lPua2n+awtOWVhuS4muW5v+WcuuaMh+WumuS4k+iQpeW6lzwvZGVwdF9uYW1lPgogICAgICAgICAgICAgICAgPGRpc3RfY29kZT4xMjEwNDIzMDwvZGlzdF9jb2RlPgogICAgICAgICAgICAgICAgPGRpc3RfbmFtZT7mm77ljY7vvIjnu4/plIDllYbvvIl0ZXN0PC9kaXN0X25hbWU+CiAgICAgICAgICAgICAgICA8b3JkZXJfbW9uZXk+MjA8L29yZGVyX21vbmV5PgogICAgICAgICAgICAgICAgPGRlbGl2X25hbWU+5reu5a6J5ZCI5L2c5Y6FMTwvZGVsaXZfbmFtZT4KICAgICAgICAgICAgICAgIDxkZWxpdl9tb2JpbGU+MTM5MDUxOTQ1ODE8L2RlbGl2X21vYmlsZT4KICAgICAgICAgICAgICAgIDxkZWxpdl9hZGRyZXNzPuS4reWkrui3rzQw5Y+3MTA25Y2V5YWDPC9kZWxpdl9hZGRyZXNzPgogICAgICAgICAgICAgICAgPGlzX21lZXRpbmc+MDwvaXNfbWVldGluZz4KICAgICAgICAgICAgICAgIDxtZWV0aW5nX25hbWU+PC9tZWV0aW5nX25hbWU+CiAgICAgICAgICAgICAgICA8cGF5X21vZGU+MTwvcGF5X21vZGU+CiAgICAgICAgICAgICAgICA8YWxsX3BheV9tb25leT4yMDwvYWxsX3BheV9tb25leT4KICAgICAgICAgICAgICAgIDxtdWx0aXBseV9wYXk+CiAgICAgICAgICAgICAgICAgICAgPHBheT4KICAgICAgICAgICAgICAgICAgICAgICAgPHBheV90aW1lPjIwMTgwMzEyMTUyMzE4PC9wYXlfdGltZT4KICAgICAgICAgICAgICAgICAgICAgICAgPHBheV9tb25leT4yMDwvcGF5X21vbmV5PgogICAgICAgICAgICAgICAgICAgICAgICA8cGF5X3R5cGU+RkxLPC9wYXlfdHlwZT4KICAgICAgICAgICAgICAgICAgICA8L3BheT4KICAgICAgICAgICAgICAgIDwvbXVsdGlwbHlfcGF5PgogICAgICAgICAgICAgICAgPGl0ZW1fbGlzdD4KICAgICAgICAgICAgICAgICAgICA8aXRlbT4KICAgICAgICAgICAgICAgICAgICAgICAgPGRldGFpbF9udW0+UzIwMTgwMzEyMTUxMzI4NzIwNjA0PC9kZXRhaWxfbnVtPgogICAgICAgICAgICAgICAgICAgICAgICA8Z29vZHNfdHlwZT5TUExYX1NKWkQ8L2dvb2RzX3R5cGU+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19udW0+SlNZRC1ERFNDU1BQLUREU0NTWEg2LTAyPC9nb29kc19udW0+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19wcmljZT4xMDwvZ29vZHNfcHJpY2U+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnb29kc19jb3VudD4yPC9nb29kc19jb3VudD4KICAgICAgICAgICAgICAgICAgICA8L2l0ZW0+CiAgICAgICAgICAgICAgICA8L2l0ZW1fbGlzdD4KICAgICAgICAgICAgICAgIDxnaWZ0X2xpc3QvPgogICAgICAgICAgICA8L29yZGVyPgogICAgICAgIDwvb3JkZXJfbGlzdHM+CiAgICA8L2JvZHk+CjwvcmVxdWVzdD4="));
            var dt = DateTime.Now;
            //using (SQLServerAccess sq = new SQLServerAccess())
            //{
            //    sq.Open("Password=sa;Persist Security Info=True;User ID=sa;Initial Catalog=ChuYuWang_UC;Data Source=.;pooling=true;connection lifetime=0;min pool size = 1;max pool size=2000");
            //    Console.WriteLine($"sqlserver open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //    var sql = "select * from CodeDictionary";
            //    DBOParameterCollection dpc = new DBOParameterCollection();
            //    var result = sq.Query(sql, dpc);
            //    Console.WriteLine($"sqlserver cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //    result = sq.Query(sql, dpc);
            //    Console.WriteLine($"sqlserver2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //}
            //Console.WriteLine($"sqlserver cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //using (SqliteAccess sq = new SqliteAccess())
            //{
            //    sq.Open("Data Source=./AppData/db_jiangsu.db;");
            //    Console.WriteLine($"sqlite open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //    var sql = "SELECT  COUNT(1) FROM orders AS orders WHERE orders.build_time LIKE '%'||$4432";
            //    DBOParameterCollection dpc = new DBOParameterCollection();
            //    dpc.SetValue("4432", "20180309");
            //    var result = sq.Query(sql, dpc);
            //    Console.WriteLine($"sqlite cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //    result = sq.Query(sql, dpc);
            //    Console.WriteLine($"sqlite2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //}
            //Console.WriteLine($"sqlite cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            using (MySQLAccess sq = new MySQLAccess())
            {

                sq.Open("server=10.15.1.240;user id=root;password=111111;database=ptac_b2b;charset=utf8;Convert Zero Datetime=true;Allow Zero Datetime=true;");
                Console.WriteLine($"1:open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                var s = from t in sq.NewLinqTable("admin_user", "a")
                        select t;
                var sql = s.ToSql();
                DBOParameterCollection dpc = new DBOParameterCollection();

                //dpc.SetValue("id", 1);
                var result = sq.Query(sql, dpc);
                Console.WriteLine($"1:query cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                dpc.SetValue("userid", "167");
                result = sq.Query(sql + " where user_id=@userid", dpc);
                Console.WriteLine($"1:query2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }

            //using (MySQLAccess sq = new MySQLAccess())
            //{

            //    sq.Open("server=10.15.1.240;user id=root;password=111111;database=ptac_shop;charset=utf8;Convert Zero Datetime=true;Allow Zero Datetime=true;");
            //    Console.WriteLine($"2:open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //    var sql = "SELECT operating_unit_id FROM ecs_admin_user ";
            //    DBOParameterCollection dpc = new DBOParameterCollection();

            //    //dpc.SetValue("id", 1);
            //    var result = sq.Query(sql, dpc);
            //    Console.WriteLine($"2:query cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            //}

        }
    }
}
