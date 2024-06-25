<template>
	<CommonPageHeader></CommonPageHeader>
	<slot />
	<CommonPageFooter></CommonPageFooter>
	<el-backtop :right="30" :bottom="30" />
</template>