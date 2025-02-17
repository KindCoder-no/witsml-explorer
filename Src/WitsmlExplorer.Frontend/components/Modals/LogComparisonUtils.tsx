import LogCurveInfo from "../../models/logCurveInfo";

export interface Indexes {
  mnemonic: string;
  sourceStart: string | number;
  targetStart: string | number;
  sourceEnd: string | number;
  targetEnd: string | number;
}

export const missingIndex = "-";

function logCurveInfoToIndexes(sourceLogCurveInfo?: LogCurveInfo, targetLogCurveInfo?: LogCurveInfo): Indexes {
  return {
    mnemonic: sourceLogCurveInfo ? sourceLogCurveInfo.mnemonic : targetLogCurveInfo.mnemonic,
    sourceStart: getStartIndex(sourceLogCurveInfo),
    targetStart: getStartIndex(targetLogCurveInfo),
    sourceEnd: getEndIndex(sourceLogCurveInfo),
    targetEnd: getEndIndex(targetLogCurveInfo)
  };
}

function getStartIndex(logCurveInfo?: LogCurveInfo): string | number {
  if (!logCurveInfo) {
    return missingIndex;
  }
  if (logCurveInfo.minDepthIndex != null) {
    return logCurveInfo.minDepthIndex;
  }
  if (logCurveInfo.minDateTimeIndex != null) {
    return logCurveInfo.minDateTimeIndex;
  }
  return missingIndex;
}

function getEndIndex(logCurveInfo?: LogCurveInfo): string | number {
  if (!logCurveInfo) {
    return missingIndex;
  }
  if (logCurveInfo.maxDepthIndex != null) {
    return logCurveInfo.maxDepthIndex;
  }
  if (logCurveInfo.maxDateTimeIndex != null) {
    return logCurveInfo.maxDateTimeIndex;
  }
  return missingIndex;
}

function areMismatched(sourceLogCurveInfo: LogCurveInfo, targetLogCurveInfo: LogCurveInfo): boolean {
  return (
    sourceLogCurveInfo.minDateTimeIndex != targetLogCurveInfo.minDateTimeIndex ||
    sourceLogCurveInfo.maxDateTimeIndex != targetLogCurveInfo.maxDateTimeIndex ||
    sourceLogCurveInfo.minDepthIndex != targetLogCurveInfo.minDepthIndex ||
    sourceLogCurveInfo.maxDepthIndex != targetLogCurveInfo.maxDepthIndex
  );
}

export function calculateMismatchedIndexes(sourceLogCurveInfo: LogCurveInfo[], targetLogCurveInfo: LogCurveInfo[]): Indexes[] {
  const mismatchedIndexes = [];

  for (const sourceCurve of sourceLogCurveInfo) {
    const targetCurve = targetLogCurveInfo.find((targetCurve) => targetCurve.mnemonic == sourceCurve.mnemonic);
    if (!targetCurve || areMismatched(sourceCurve, targetCurve)) {
      mismatchedIndexes.push(logCurveInfoToIndexes(sourceCurve, targetCurve));
    }
  }
  for (const targetCurve of targetLogCurveInfo) {
    const sourceCurve = sourceLogCurveInfo.find((sourceCurve) => sourceCurve.mnemonic == targetCurve.mnemonic);
    if (!sourceCurve) {
      mismatchedIndexes.push(logCurveInfoToIndexes(sourceCurve, targetCurve));
    }
  }
  return mismatchedIndexes;
}

export function markNumberDifferences(string1: string, string2: string): (string | JSX.Element)[][] {
  if (string1 == missingIndex || string2 == missingIndex) {
    return [[<mark key="1">{string1}</mark>], [<mark key="2">{string2}</mark>]];
  }
  const string1Inverse = string1.split("").reverse();
  const string2Inverse = string2.split("").reverse();
  let previousDifferent = false;
  const string1Parts = [];
  const string2Parts = [];
  let string1Part = "";
  let string2Part = "";
  for (let i = 0; i < Math.min(string1Inverse.length, string2Inverse.length); i++) {
    const currentDifferent = string1Inverse[i] !== string2Inverse[i];
    const splitString = (currentDifferent && !previousDifferent) || (!currentDifferent && previousDifferent);
    if (splitString) {
      string1Parts.push(string1Part.split("").reverse().join(""));
      string2Parts.push(string2Part.split("").reverse().join(""));
      string1Part = "";
      string2Part = "";
      previousDifferent = currentDifferent;
    }
    string1Part = string1Part.concat(string1Inverse[i]);
    string2Part = string2Part.concat(string2Inverse[i]);
  }

  string1Parts.push(string1Part.split("").reverse().join(""));
  string2Parts.push(string2Part.split("").reverse().join(""));

  const result1 = string1Parts.map((part, index) => {
    return index % 2 == 0 ? part : <mark key={index}>{part}</mark>;
  });
  const result2 = string2Parts.map((part, index) => {
    return index % 2 == 0 ? part : <mark key={index}>{part}</mark>;
  });
  return [result1.reverse(), result2.reverse()];
}

export function markDateTimeStringDifferences(string1: string, string2: string): (string | JSX.Element)[][] {
  if (string1 == missingIndex || string2 == missingIndex) {
    return [[<mark key="1">{string1}</mark>], [<mark key="2">{string2}</mark>]];
  }
  const parts1 = splitDateTimeString(string1);
  const parts2 = splitDateTimeString(string2);
  const result1 = [];
  const result2 = [];
  for (let i = 0; i < Math.min(parts1.length, parts2.length); i++) {
    if (parts1[i] != parts2[i]) {
      result1.push(<mark key={i}>{parts1[i]}</mark>);
      result2.push(<mark key={i}>{parts2[i]}</mark>);
    } else {
      result1.push(parts1[i]);
      result2.push(parts2[i]);
    }
  }
  return [result1, result2];
}

function splitDateTimeString(dateTime: string) {
  //split an ISO 8601 string into groups that should be marked together
  //the regex includes separators in the result to simplify putting the string back together
  return dateTime.split(/(?=[:T.\-Z])|(?<=[:T.\-Z])/g);
}
