namespace Lilly.PlantsPatch2

{
    public enum PlantEnum
    {
        growDays, // 성장일
        fertilityMin,// 최소 비옥도
        fertilitySensitivity, // 토양 민감도
        sowWork,//심는 작업량
        harvestWork, // 수확량
        harvestYield, // 수확량
        //harvestTag, // 수확 종류
        growMinGlow,//최소 광량
        growOptimalGlow, // 최고 광량 0~100
        harvestMinGrowth,
        harvestAfterGrowth, // 수확 가능 성장율
        minGrowthTemperature,
        minOptimalGrowthTemperature,
        maxGrowthTemperature,
        maxOptimalGrowthTemperature,
        minSpacingBetweenSamePlant,// 최소 간격
        lifespanDaysPerGrowDays, // 수명
        plantRespawningCommonalityFactor,// 재생성 확률 0~1
        cavePlantWeight,    // 동굴 확률
        topWindExposure, // 바람에 흔들리는 정도 0~1
        wildClusterWeight,// 군집 확률 0~1
    }
}
