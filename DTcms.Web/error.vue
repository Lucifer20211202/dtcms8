<template>
	<el-row justify="center">
		<el-col :xs="12" :lg="12">
			<el-result icon="warning"
				:title="`访问出错:${error.statusCode}`"
				:sub-title="error.message"
			>
				<template #extra>
					<el-button type="primary" @click="handleError">Back</el-button>
				</template>
			</el-result>
		</el-col>
	</el-row>
</template>

<script setup>
	const props = defineProps({ error: Object })
	const handleError = () => clearError({ redirect: '/' })
</script>