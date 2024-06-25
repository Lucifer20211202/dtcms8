<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员明细'},{title:'积分记录'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="会员账户" v-if="datas.model.id>0">
							{{datas.model.userName}}
						</el-form-item>
						<el-form-item prop="userId" label="会员账户" v-else>
							<dt-member-select v-model="datas.model.userId"></dt-member-select>
						</el-form-item>
						<el-form-item label="账户积分" v-if="datas.model.id>0">
							{{datas.model.currPoint}} 元
						</el-form-item>
						<el-form-item label="操作积分" v-if="datas.model.id>0">
							{{datas.model.value}}
						</el-form-item>
						<el-form-item prop="value" label="操作积分" v-else>
							<el-input v-model="datas.model.value" placeholder="必填，负数扣减正数增加">
								<template #append>分</template>
							</el-input>
						</el-form-item>
						<el-form-item label="备注说明" v-if="datas.model.id>0">
							{{datas.model.description}}
						</el-form-item>
						<el-form-item prop="description" label="消息内容" v-else>
							<el-input type="textarea" :rows="5" v-model="datas.model.description" placeholder="必填，500字符内"></el-input>
						</el-form-item>
						<el-form-item label="操作时间" v-if="datas.model.id>0">
							{{datas.model.addTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtMemberSelect from '../../../components/control/DtMemberSelect.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			id: 0,
			userId: 0,
			userName: null,
			currPoint: null,
			value: null,
			description: null,
			addTime: null
		},
		rules: {
			userId: [
				{ required: true, message: '请输入选择账户', trigger: 'blur' }
			],
			value: [
				{ required: true, message: '请输入增减积分', trigger: 'blur' },
				{ pattern: /^-?[1-9]\d*$/, message: '经验值只能为整数', trigger: 'blur' }
			],
			description: [
				{ required: true, message: '请输入备注说明', trigger: 'blur' }
			],
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/member/point/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				proxy.$api.request({
					method: 'post',
					url: '/admin/member/point',
					data: datas.model,
					successMsg: '积分记录添加成功',
					beforeSend() {
						datas.loading = true
					},
					success(res) {
						proxy.$common.linkUrl('/member/point/list')
					},
					complete() {
						datas.loading = false
					}
				})
			}
		})
	}
	
	//运行初始化
	initData()
</script>