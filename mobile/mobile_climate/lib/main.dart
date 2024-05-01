import 'package:flutter/material.dart';


void main() {
  runApp(const MyApp());
}

class MyApp extends StatefulWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  ThemeMode _themeMode = ThemeMode.system;

  void _toggleTheme(ThemeMode themeMode) {
    setState((){
      print("tapped");
      _themeMode = themeMode;
    });
  }

  @override
  Widget build(BuildContext context) {
    bool isDarkMode = Theme.of(context).brightness == Brightness.dark;

    return MaterialApp(
      title: 'Climate',
      theme: MyAppThemes.lightTheme,
      darkTheme: MyAppThemes.darkTheme,
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        appBar: AppBar(
          title: const Text("Climate"),
          flexibleSpace: Container(
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: <Color>[
                    Color.fromRGBO(101, 130, 245, 1.0),
                    Color.fromRGBO(87, 108, 187, 1.0),
                  ]),
            ),
          ),
        ),
        drawer: SideBarMenu(),
        body: Container(
          child: ElevatedButton(
            onPressed: () {
              _toggleTheme(ThemeMode.dark);
            },
            child: Text("Hello"),
          ),
        ),
      ),
    );
  }
}

class SideBarMenu extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              Color.fromRGBO(101, 130, 245, 1.0),
              Color.fromRGBO(87, 108, 187, 1.0),
              Color.fromRGBO(49, 73, 162, 1.0),
              Color.fromRGBO(25, 45, 124, 1.0)
            ],
          ),
        ),
      ),
    );
  }
}

class MyAppColors {
  static final darkBlue = Color(0xFF0000DE);
  static final lightBlue = Color(0xFFFF00C3);
}

class MyAppThemes {
  static final darkTheme = ThemeData(
    primaryColor: MyAppColors.darkBlue,
    brightness: Brightness.dark,
  );

  static final lightTheme = ThemeData(
    primaryColor: MyAppColors.lightBlue,
    brightness: Brightness.light,
  );
}
