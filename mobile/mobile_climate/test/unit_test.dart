import 'package:flutter_test/flutter_test.dart';
import 'package:mobile_climate/main.dart';

void main() {
  test('Is Even', () {
    bool result = isEven(12);
    expect(result, true);
  });
  test('Is Odd', () {
    bool result = isEven(123);
    expect(result, false);
  });
  test('Is Evener', () {
    bool result = isEven(122);
    expect(result, true);
  });
  test('Is Odder', () {
    bool result = isEven(123);
    expect(result, false);
  });
}